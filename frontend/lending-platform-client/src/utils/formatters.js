export const formatCurrency = (value) => {
  if (value === null || value === undefined) return '-';
  
  return new Intl.NumberFormat('en-GB', {
    style: 'currency',
    currency: 'GBP',
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(value);
};

export const formatLtv = (ltv) => {
  if (ltv === null || ltv === undefined) return '-';
  // LTV is passed as a percentage value, e.g. 50 meaning 50%
  return `${Number(ltv).toFixed(2)}%`;
};

export const formatDate = (dateString) => {
  if (!dateString) return '-';
  const d = new Date(dateString);
  return d.toLocaleString('en-GB', {
    day: '2-digit',
    month: 'short',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  });
};
