import axios from 'axios';
import type { AdminStats, ApplicationRecord, AuthResponse, CandidateApplication, Job, RecruiterJob, SeekerProfile } from '../types';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL ?? 'http://localhost:5000/api',
  timeout: 12000,
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('careerconnect_token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

export const getApiError = (error: unknown): string => {
  if (axios.isAxiosError(error)) {
    return error.response?.data?.message ?? error.response?.data?.title ?? error.message;
  }
  return error instanceof Error ? error.message : 'An unexpected error occurred.';
};

export const getJobs = async (): Promise<Job[]> => {
  const response = await api.get('/jobs');
  const list = response.data?.items ?? response.data ?? [];
  return Array.isArray(list) ? list : [];
};

export const login = async (email: string, password: string): Promise<AuthResponse> => {
  const response = await api.post('/auth/login', { email, password });
  return response.data;
};

export const register = async (payload: Record<string, string>): Promise<AuthResponse> => {
  const response = await api.post('/auth/register', payload);
  return response.data;
};

export const createJob = async (payload: Record<string, unknown>) => {
  const response = await api.post('/jobs', payload);
  return response.data;
};

export const applyToJob = async (jobId: number) => {
  const response = await api.post(`/jobs/${jobId}/apply`);
  return response.data;
};

export const getMyApplications = async (): Promise<ApplicationRecord[]> => {
  const response = await api.get('/applications/my');
  return response.data;
};

export const getMyJobs = async (): Promise<RecruiterJob[]> => {
  const response = await api.get('/jobs/mine');
  return response.data;
};

export const getJobApplications = async (jobId: number): Promise<CandidateApplication[]> => {
  const response = await api.get(`/jobs/${jobId}/applications`);
  return response.data;
};

export const updateApplicationStatus = async (applicationId: number, status: string) => {
  const response = await api.put(`/applications/${applicationId}/status`, { status });
  return response.data;
};

export const getMyProfile = async (): Promise<SeekerProfile> => {
  const response = await api.get('/profile');
  return response.data;
};

export const updateMyProfile = async (profile: SeekerProfile) => {
  const response = await api.put('/profile', profile);
  return response.data;
};

export const getAdminStats = async (): Promise<AdminStats> => {
  const response = await api.get('/admin/statistics');
  return response.data;
};

export default api;
