import axios from 'axios';

// Create a configured axios instance
// The Vite proxy running on port 5173 points relative requests to the backend.
const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '/', // Use environment variable or fallback to same-domain proxy
  withCredentials: true, // This is CRITICAL for sending HttpOnly cookies!
});

export default api;
