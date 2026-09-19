import React, { useState, useMemo, useCallback } from 'react';
import { formatCurrency, formatLtv, formatDate } from '../utils/formatters';
import { RefreshCw, Search, Download, Info, CheckCircle2, XCircle } from 'lucide-react';

/* ── Detail Modal ─────────────────────────────────────── */
const DetailModal = ({ app, onClose }) => {
  if (!app) return null;

  const isApproved = app.decision === 'Approved';

  // Close on overlay click
  const handleOverlayClick = (e) => {
    if (e.target === e.currentTarget) onClose();
  };

  // Close on Escape
  const handleKeyDown = useCallback((e) => {
    if (e.key === 'Escape') onClose();
  }, [onClose]);

  return (
    <div
      className="modal-overlay"
      role="dialog"
      aria-modal="true"
      aria-labelledby="modal-title"
      onClick={handleOverlayClick}
      onKeyDown={handleKeyDown}
      tabIndex={-1}
    >
      <div className="modal-panel">
        <div className="modal-header">
          <h3 id="modal-title">Application #A{app.id}</h3>
          <button className="modal-close" onClick={onClose} aria-label="Close">×</button>
        </div>

        <div className="modal-body">
          <div
            className={`modal-decision-badge modal-decision-badge--${isApproved ? 'approved' : 'declined'}`}
          >
            {isApproved
              ? <CheckCircle2 size={14} aria-hidden="true" />
              : <XCircle size={14} aria-hidden="true" />}
            {app.decision}
          </div>

          <dl className="modal-dl">
            <div>
              <dt>Date</dt>
              <dd>{formatDate(app.createdAt)}</dd>
            </div>
            <div>
              <dt>Loan Amount</dt>
              <dd>{formatCurrency(app.loanAmount)}</dd>
            </div>
            <div>
              <dt>Asset Value</dt>
              <dd>{formatCurrency(app.assetValue)}</dd>
            </div>
            <div>
              <dt>Credit Score</dt>
              <dd>{app.creditScore}</dd>
            </div>
            <div>
              <dt>Calculated LTV</dt>
              <dd>{formatLtv(app.ltv)}</dd>
            </div>
          </dl>

          <div className="modal-reason">
            <p className="reason-label">Reason for decision</p>
            <p className="reason-text">{app.decisionReason}</p>
            <span className="rule-badge">{app.ruleApplied}</span>
          </div>
        </div>

        <div className="modal-footer">
          <button className="btn btn-secondary" onClick={onClose}>Close</button>
        </div>
      </div>
    </div>
  );
};

/* ── Policy Drawer ────────────────────────────────────── */
const PolicyDrawer = ({ onClose }) => {
  const handleOverlayClick = (e) => {
    if (e.target === e.currentTarget) onClose();
  };

  const handleKeyDown = useCallback((e) => {
    if (e.key === 'Escape') onClose();
  }, [onClose]);

  return (
    <>
      <div
        className="drawer-overlay"
        onClick={handleOverlayClick}
        onKeyDown={handleKeyDown}
        tabIndex={-1}
        aria-hidden="true"
      />
      <div
        className="drawer-panel"
        role="complementary"
        aria-label="Lending Policy"
      >
        <div className="drawer-header">
          <h2>Lending Policy</h2>
          <button className="modal-close" onClick={onClose} aria-label="Close Lending Policy">×</button>
        </div>

        <div className="drawer-body">
          <div className="policy-section">
            <h3>Standard Loans (&lt; £1,000,000)</h3>
            <ul className="policy-rules">
              <li className="policy-rule">
                <span className="policy-rule-arrow">›</span>
                <span>LTV &lt; 60% → Score ≥ 750</span>
              </li>
              <li className="policy-rule">
                <span className="policy-rule-arrow">›</span>
                <span>60% ≤ LTV &lt; 80% → Score ≥ 800</span>
              </li>
              <li className="policy-rule">
                <span className="policy-rule-arrow">›</span>
                <span>80% ≤ LTV &lt; 90% → Score ≥ 900</span>
              </li>
              <li className="policy-rule">
                <span className="policy-rule-arrow">›</span>
                <span>LTV ≥ 90% → Declined</span>
              </li>
            </ul>
          </div>

          <div className="policy-section">
            <h3>High-Value Loans (≥ £1,000,000)</h3>
            <ul className="policy-rules">
              <li className="policy-rule">
                <span className="policy-rule-arrow">›</span>
                <span>LTV ≤ 60% AND Score ≥ 950</span>
              </li>
            </ul>
          </div>

          <div className="policy-section">
            <h3>Hard Limits</h3>
            <ul className="policy-rules">
              <li className="policy-rule">
                <span className="policy-rule-arrow">›</span>
                <span>Loan &lt; £100,000 → Declined</span>
              </li>
              <li className="policy-rule">
                <span className="policy-rule-arrow">›</span>
                <span>Loan &gt; £1,500,000 → Declined</span>
              </li>
            </ul>
          </div>

          <p style={{ fontSize: '0.75rem', color: 'var(--text-muted)', marginTop: '1.5rem', lineHeight: '1.5' }}>
            This information is for explanation only. DO NOT duplicate or implement decision logic in the frontend. The backend remains authoritative.
          </p>
        </div>
      </div>
    </>
  );
};

/* ── Main History Component ───────────────────────────── */
const ApplicationHistory = ({ applications, onRefresh, onFilterChange, currentFilter, isLoading }) => {
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedApp, setSelectedApp] = useState(null);
  const [showPolicy, setShowPolicy] = useState(false);

  const displayed = useMemo(() => {
    if (!searchTerm.trim()) return applications;
    const term = searchTerm.trim().toLowerCase().replace(/^a/, '');
    return applications.filter(
      (app) => app.id.toString().includes(term) || `a${app.id}`.includes(searchTerm.trim().toLowerCase())
    );
  }, [applications, searchTerm]);

  const handleExport = () => {
    if (displayed.length === 0) return;
    const headers = ['Application ID', 'Date', 'Loan Amount (GBP)', 'Asset Value (GBP)', 'Credit Score', 'LTV (%)', 'Decision', 'Reason', 'Rule Applied'];
    const rows = displayed.map((a) => [
      `A${a.id}`,
      new Date(a.createdAt).toISOString(),
      a.loanAmount,
      a.assetValue,
      a.creditScore,
      Number(a.ltv).toFixed(2),
      a.decision,
      `"${a.decisionReason.replace(/"/g, '""')}"`,
      `"${a.ruleApplied}"`,
    ]);
    const csv = [headers.join(','), ...rows.map((r) => r.join(','))].join('\n');
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `lendcore-${new Date().toISOString().split('T')[0]}.csv`;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
  };

  const colSpan = 7;

  return (
    <>
      <div className="card history-section">
        {/* Toolbar */}
        <div className="history-toolbar">
          <h2 className="history-title">Application History</h2>

          {/* Search */}
          <div className="search-wrap">
            <Search size={14} className="search-icon" aria-hidden="true" />
            <input
              type="search"
              className="search-input"
              placeholder="Search ID…"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              aria-label="Search by application ID"
            />
          </div>

          {/* Filter */}
          <select
            className="filter-select"
            value={currentFilter}
            onChange={(e) => onFilterChange(e.target.value)}
            aria-label="Filter by decision"
          >
            <option value="All">All decisions</option>
            <option value="Approved">Approved</option>
            <option value="Declined">Declined</option>
          </select>

          {/* Policy */}
          <button
            className="policy-btn"
            onClick={() => setShowPolicy(true)}
            aria-label="Open lending policy"
          >
            <Info size={14} aria-hidden="true" />
            Lending Policy
          </button>

          {/* Refresh */}
          <button
            className="btn-icon"
            onClick={onRefresh}
            disabled={isLoading}
            aria-label={isLoading ? 'Refreshing…' : 'Refresh history'}
            title="Refresh"
          >
            <RefreshCw size={15} className={isLoading ? 'spin' : ''} aria-hidden="true" />
          </button>

          {/* Export */}
          <button
            className="btn-icon"
            onClick={handleExport}
            disabled={displayed.length === 0}
            aria-label="Export as CSV"
            title="Export CSV"
          >
            <Download size={15} aria-hidden="true" />
          </button>
        </div>

        {/* Table */}
        <div className="table-wrap">
          <table className="data-table" aria-label="Loan application history">
            <thead>
              <tr>
                <th scope="col">ID</th>
                <th scope="col">Date</th>
                <th scope="col">Loan Amount</th>
                <th scope="col">Asset Value</th>
                <th scope="col">LTV</th>
                <th scope="col">Score</th>
                <th scope="col">Decision</th>
                <th scope="col"><span className="sr-only">Actions</span></th>
              </tr>
            </thead>
            <tbody>
              {isLoading && applications.length === 0 ? (
                <tr className="loading-row">
                  <td colSpan={colSpan + 1}>Loading history…</td>
                </tr>
              ) : displayed.length === 0 ? (
                <tr className="empty-row">
                  <td colSpan={colSpan + 1}>
                    <span className="empty-row-title">No applications yet</span>
                    Submit your first loan application to see it appear here.
                  </td>
                </tr>
              ) : (
                displayed.map((app) => (
                  <tr key={app.id}>
                    <td className="col-id">#A{app.id}</td>
                    <td className="col-date">{formatDate(app.createdAt)}</td>
                    <td>{formatCurrency(app.loanAmount)}</td>
                    <td>{formatCurrency(app.assetValue)}</td>
                    <td>{formatLtv(app.ltv)}</td>
                    <td className="col-score">{app.creditScore}</td>
                    <td>
                      <span
                        className={`badge badge--${app.decision === 'Approved' ? 'approved' : 'declined'}`}
                        aria-label={`Decision: ${app.decision}`}
                      >
                        {app.decision === 'Approved'
                          ? <CheckCircle2 size={11} aria-hidden="true" />
                          : <XCircle size={11} aria-hidden="true" />}
                        {app.decision}
                      </span>
                    </td>
                    <td>
                      <button
                        className="view-details-btn"
                        onClick={() => setSelectedApp(app)}
                        aria-label={`View details for application A${app.id}`}
                      >
                        View details
                      </button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>

      {/* Detail Modal */}
      {selectedApp && <DetailModal app={selectedApp} onClose={() => setSelectedApp(null)} />}

      {/* Policy Drawer */}
      {showPolicy && <PolicyDrawer onClose={() => setShowPolicy(false)} />}
    </>
  );
};

export default ApplicationHistory;
