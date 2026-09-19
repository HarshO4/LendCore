import React from 'react';
import { formatCurrency, formatLtv } from '../utils/formatters';
import { Users, CheckCircle, XCircle, PoundSterling, Percent } from 'lucide-react';

const cards = (stats) => [
  {
    label: 'Total Applicants',
    value: stats.totalApplicants,
    icon: <Users size={18} aria-hidden="true" />,
    iconClass: 'stat-icon--neutral',
  },
  {
    label: 'Approved',
    value: stats.approvedCount,
    icon: <CheckCircle size={18} aria-hidden="true" />,
    iconClass: 'stat-icon--success',
  },
  {
    label: 'Declined',
    value: stats.declinedCount,
    icon: <XCircle size={18} aria-hidden="true" />,
    iconClass: 'stat-icon--danger',
  },
  {
    label: 'Loans Written',
    value: formatCurrency(stats.totalValueOfApprovedLoans),
    icon: <PoundSterling size={18} aria-hidden="true" />,
    iconClass: 'stat-icon--neutral',
  },
  {
    label: 'Average LTV',
    value: stats.averageLtv != null ? formatLtv(stats.averageLtv) : '—',
    icon: <Percent size={18} aria-hidden="true" />,
    iconClass: 'stat-icon--neutral',
  },
];

const StatsCards = ({ stats }) => {
  if (!stats) return null;

  return (
    <div className="stats-grid" role="list" aria-label="Portfolio statistics">
      {cards(stats).map((card) => (
        <div key={card.label} className="stat-card" role="listitem">
          <div className={`stat-icon ${card.iconClass}`} aria-hidden="true">
            {card.icon}
          </div>
          <div>
            <p className="stat-label">{card.label}</p>
            <p className="stat-value">{card.value}</p>
          </div>
        </div>
      ))}
    </div>
  );
};

export default StatsCards;
