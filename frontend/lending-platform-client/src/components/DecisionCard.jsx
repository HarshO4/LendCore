import React, { useState } from 'react';
import { formatCurrency, formatLtv } from '../utils/formatters';
import { CheckCircle2, XCircle, Copy, Check, X } from 'lucide-react';

const DecisionCard = ({ decision, onClear, phase }) => {
  const [copied, setCopied] = useState(false);

  // phase = 'assessing' | 'flash' | 'result' | null
  if (phase === 'assessing') {
    return (
      <div className="assessing-state" role="status" aria-live="polite">
        <svg
          className="spin"
          width="28"
          height="28"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          strokeWidth="2"
          strokeLinecap="round"
          aria-hidden="true"
        >
          <path d="M12 2v4M12 18v4M4.93 4.93l2.83 2.83M16.24 16.24l2.83 2.83M2 12h4M18 12h4M4.93 19.07l2.83-2.83M16.24 7.76l2.83-2.83" />
        </svg>
        <span>Assessing application…</span>
      </div>
    );
  }

  if (phase === 'flash' && decision) {
    const approved = decision.decision === 'Approved';
    return (
      <div className={`decision-flash decision-flash--${approved ? 'approved' : 'declined'}`} role="status" aria-live="assertive">
        <div className="flash-icon" aria-hidden="true">
          {approved
            ? <CheckCircle2 size={52} color="var(--success)" />
            : <XCircle size={52} color="var(--danger)" />}
        </div>
        <span className={`flash-label flash-label--${approved ? 'approved' : 'declined'}`}>
          {decision.decision}
        </span>
      </div>
    );
  }

  if (!decision) {
    return (
      <div className="empty-decision" role="region" aria-label="Latest decision">
        <div className="empty-decision-inner">
          <svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" color="var(--text-muted)" aria-hidden="true">
            <rect x="3" y="3" width="18" height="18" rx="2" />
            <path d="M3 9h18M9 21V9" />
          </svg>
          <p>Submit an application to see the decision here.</p>
        </div>
      </div>
    );
  }

  const isApproved = decision.decision === 'Approved';

  const handleCopy = () => {
    navigator.clipboard.writeText(`A${decision.id}`);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  return (
    <div
      className={`card decision-card decision-animate`}
      role="region"
      aria-label="Latest decision"
    >
      {/* Accent header */}
      <div className={`decision-accent decision-accent--${isApproved ? 'approved' : 'declined'}`}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
          <div>
            <div className={`decision-badge decision-badge--${isApproved ? 'approved' : 'declined'}`}>
              {isApproved
                ? <CheckCircle2 size={18} aria-hidden="true" />
                : <XCircle size={18} aria-hidden="true" />}
              {decision.decision}
            </div>

            <div className="decision-app-id">
              <span>Application #A{decision.id}</span>
              <button
                onClick={handleCopy}
                className={`copy-btn${copied ? ' copy-btn--copied' : ''}`}
                aria-label={copied ? 'Copied' : 'Copy application ID'}
                title="Copy ID"
              >
                {copied ? <Check size={12} aria-hidden="true" /> : <Copy size={12} aria-hidden="true" />}
                {copied ? 'Copied' : 'Copy ID'}
              </button>
            </div>
          </div>

          {onClear && (
            <button
              onClick={onClear}
              className="clear-decision-btn"
              aria-label="Clear decision"
              title="Clear decision"
            >
              <X size={16} aria-hidden="true" />
            </button>
          )}
        </div>
      </div>

      {/* Body */}
      <div className="decision-body">
        <dl className="dl-grid">
          <div className="dl-item">
            <dt>Loan Amount</dt>
            <dd>{formatCurrency(decision.loanAmount)}</dd>
          </div>
          <div className="dl-item">
            <dt>Asset Value</dt>
            <dd>{formatCurrency(decision.assetValue)}</dd>
          </div>
          <div className="dl-item">
            <dt>Credit Score</dt>
            <dd>{decision.creditScore}</dd>
          </div>
          <div className="dl-item">
            <dt>Calculated LTV</dt>
            <dd className="ltv-value">{formatLtv(decision.ltv)}</dd>
          </div>
        </dl>

        <div className="reason-section">
          <p className="reason-label">Reason for decision</p>
          <p className="reason-text">{decision.decisionReason}</p>
          <span className="rule-badge">{decision.ruleApplied}</span>
        </div>
      </div>
    </div>
  );
};

export default DecisionCard;
