import React, { useState, useEffect, useCallback } from 'react';
import Header from './components/Header';
import StatsCards from './components/StatsCards';
import ApplicationForm from './components/ApplicationForm';
import DecisionCard from './components/DecisionCard';
import ApplicationHistory from './components/ApplicationHistory';
import { submitApplication, getApplications, getStatistics, checkApiHealth } from './services/api';

// Decision animation phases:
// null → 'assessing' → 'flash' → 'result'
const FLASH_DURATION_MS = 750;

function App() {
  const [isApiOnline, setIsApiOnline] = useState(true);
  const [stats, setStats] = useState(null);
  const [applications, setApplications] = useState([]);
  const [latestDecision, setLatestDecision] = useState(null);
  const [decisionPhase, setDecisionPhase] = useState(null); // null | 'assessing' | 'flash' | 'result'

  const [isSubmitting, setIsSubmitting] = useState(false);
  const [isLoadingHistory, setIsLoadingHistory] = useState(false);

  const [filter, setFilter] = useState('All');
  const [errorMsg, setErrorMsg] = useState('');

  const loadDashboardData = useCallback(async (currentFilter = filter) => {
    setIsLoadingHistory(true);
    try {
      const [statsData, appsData] = await Promise.all([
        getStatistics(),
        getApplications(currentFilter),
      ]);
      setStats(statsData);
      setApplications(appsData);
      setIsApiOnline(true);
      setErrorMsg('');
    } catch (err) {
      console.error('Failed to load dashboard data:', err);
      setIsApiOnline(false);
      setErrorMsg('Unable to connect to the lending decision engine. Please make sure the API is running and try again.');
    } finally {
      setIsLoadingHistory(false);
    }
  }, [filter]);

  useEffect(() => {
    loadDashboardData();

    const interval = setInterval(async () => {
      const online = await checkApiHealth();
      setIsApiOnline(online);
    }, 15000);

    return () => clearInterval(interval);
  }, [loadDashboardData]);

  const handleFilterChange = (newFilter) => {
    setFilter(newFilter);
    loadDashboardData(newFilter);
  };

  const handleApplicationSubmit = async (formData) => {
    setIsSubmitting(true);
    setErrorMsg('');
    setDecisionPhase('assessing');

    try {
      const result = await submitApplication(formData);

      // Flash the decision icon briefly
      setLatestDecision(result);
      setDecisionPhase('flash');

      await new Promise((resolve) => setTimeout(resolve, FLASH_DURATION_MS));

      setDecisionPhase('result');

      // Refresh dashboard in background
      loadDashboardData(filter);
    } catch (err) {
      console.error('Application submission failed:', err);
      setDecisionPhase(null);

      if (err.response?.data?.errors) {
        // ASP.NET validation problem details
        const messages = Object.values(err.response.data.errors).flat().join(' ');
        setErrorMsg(`Validation error: ${messages}`);
      } else if (err.response?.data?.title) {
        setErrorMsg(`Validation error: ${err.response.data.title}`);
      } else if (err.response?.status >= 500) {
        setErrorMsg('The lending decision engine returned an error. Please try again.');
      } else {
        setErrorMsg('Unable to connect to the lending decision engine. Please make sure the API is running and try again.');
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleClearDecision = () => {
    setLatestDecision(null);
    setDecisionPhase(null);
  };

  const showDecisionPanel = decisionPhase !== null || latestDecision !== null;

  return (
    <div>
      <Header isApiOnline={isApiOnline} />

      <main className="main-content">
        {/* Error banner */}
        {errorMsg && (
          <div className="error-banner" role="alert">
            {errorMsg}
          </div>
        )}

        {/* Stats */}
        <section className="stats-section" aria-label="Portfolio statistics">
          {stats ? (
            <StatsCards stats={stats} />
          ) : (
            <div className="stats-loading" aria-busy="true">Loading statistics…</div>
          )}
        </section>

        {/* Form + Decision */}
        <div className="layout-grid">
          <div>
            <ApplicationForm onSubmit={handleApplicationSubmit} isLoading={isSubmitting} />
          </div>
          <div>
            <DecisionCard
              decision={latestDecision}
              onClear={showDecisionPanel ? handleClearDecision : null}
              phase={decisionPhase === 'result' ? null : decisionPhase}
            />
          </div>
        </div>

        {/* History */}
        <ApplicationHistory
          applications={applications}
          onRefresh={() => loadDashboardData(filter)}
          onFilterChange={handleFilterChange}
          currentFilter={filter}
          isLoading={isLoadingHistory}
        />
      </main>
    </div>
  );
}

export default App;
