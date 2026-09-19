import React, { useState } from 'react';

const ApplicationForm = ({ onSubmit, isLoading }) => {
  const [formData, setFormData] = useState({ loanAmount: '', assetValue: '', creditScore: '' });
  const [errors, setErrors] = useState({});

  const validate = () => {
    const e = {};
    if (!formData.loanAmount || Number(formData.loanAmount) <= 0)
      e.loanAmount = 'Loan amount must be greater than 0.';
    if (!formData.assetValue || Number(formData.assetValue) <= 0)
      e.assetValue = 'Asset value must be greater than 0.';
    if (!formData.creditScore || Number(formData.creditScore) < 1 || Number(formData.creditScore) > 999)
      e.creditScore = 'Credit score must be between 1 and 999.';
    setErrors(e);
    return Object.keys(e).length === 0;
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
    if (errors[name]) setErrors((prev) => ({ ...prev, [name]: null }));
  };

  const handleReset = () => {
    setFormData({ loanAmount: '', assetValue: '', creditScore: '' });
    setErrors({});
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    if (validate()) {
      onSubmit({
        loanAmount: Number(formData.loanAmount),
        assetValue: Number(formData.assetValue),
        creditScore: Number(formData.creditScore),
      });
    }
  };

  return (
    <div className="card">
      <div className="card-header">
        <h2>New Application</h2>
        <p>Submit a loan application for an immediate decision.</p>
      </div>

      <form onSubmit={handleSubmit} noValidate>
        <div className="form-body">
          <div className="form-group">
            <label htmlFor="loanAmount">Loan Amount (£)</label>
            <input
              id="loanAmount"
              name="loanAmount"
              type="number"
              className={`form-input${errors.loanAmount ? ' is-invalid' : ''}`}
              value={formData.loanAmount}
              onChange={handleChange}
              placeholder="e.g. 250000"
              min="0"
              step="1"
              disabled={isLoading}
              aria-describedby={errors.loanAmount ? 'loanAmountErr' : undefined}
              aria-invalid={!!errors.loanAmount}
            />
            {errors.loanAmount && (
              <span id="loanAmountErr" className="field-error" role="alert">
                {errors.loanAmount}
              </span>
            )}
          </div>

          <div className="form-group">
            <label htmlFor="assetValue">Asset Value (£)</label>
            <input
              id="assetValue"
              name="assetValue"
              type="number"
              className={`form-input${errors.assetValue ? ' is-invalid' : ''}`}
              value={formData.assetValue}
              onChange={handleChange}
              placeholder="e.g. 500000"
              min="0"
              step="1"
              disabled={isLoading}
              aria-describedby={errors.assetValue ? 'assetValueErr' : undefined}
              aria-invalid={!!errors.assetValue}
            />
            {errors.assetValue && (
              <span id="assetValueErr" className="field-error" role="alert">
                {errors.assetValue}
              </span>
            )}
          </div>

          <div className="form-group">
            <label htmlFor="creditScore">Credit Score</label>
            <input
              id="creditScore"
              name="creditScore"
              type="number"
              className={`form-input${errors.creditScore ? ' is-invalid' : ''}`}
              value={formData.creditScore}
              onChange={handleChange}
              placeholder="e.g. 750"
              min="1"
              max="999"
              step="1"
              disabled={isLoading}
              aria-describedby="creditScoreHint"
              aria-invalid={!!errors.creditScore}
            />
            <span id="creditScoreHint" className="field-hint">Valid range: 1–999</span>
            {errors.creditScore && (
              <span className="field-error" role="alert">
                {errors.creditScore}
              </span>
            )}
          </div>

          <div className="form-actions">
            <button
              type="submit"
              className="btn btn-primary"
              disabled={isLoading}
              aria-busy={isLoading}
            >
              {isLoading ? 'Assessing…' : 'Submit Application'}
            </button>
            <button
              type="button"
              className="btn btn-secondary"
              onClick={handleReset}
              disabled={isLoading}
            >
              Reset
            </button>
          </div>
        </div>
      </form>
    </div>
  );
};

export default ApplicationForm;
