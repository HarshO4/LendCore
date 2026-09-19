import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5207/api';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
});

export const submitApplication = async (applicationData) => {
  const response = await apiClient.post('/applications', applicationData);
  return response.data;
};

export const getApplications = async (decisionFilter = null) => {
  const params = decisionFilter && decisionFilter !== 'All' ? { decision: decisionFilter } : {};
  const response = await apiClient.get('/applications', { params });
  return response.data;
};

export const getStatistics = async () => {
  const response = await apiClient.get('/statistics');
  return response.data;
};

// Check if API is online
export const checkApiHealth = async () => {
  try {
    await apiClient.get('/health');
    return true;
  } catch (error) {
    return false;
  }
};
