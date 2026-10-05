export type Job = {
  id: number;
  title: string;
  description: string;
  companyName: string;
  location: string;
  jobType: string;
  workMode: string;
  salary: string;
  experienceLevel: string;
  deadline: string;
  postedDate: string;
  vacancies: number;
  status: string;
  skills?: string[];
};

export type AuthResponse = {
  userId: number;
  email: string;
  role: string;
  token: string;
};

export type ApplicationRecord = {
  id: number;
  jobId: number;
  jobTitle: string;
  companyName: string;
  status: string;
  appliedDate: string;
};

export type RecruiterJob = {
  id: number;
  title: string;
  companyName: string;
  location: string;
  status: string;
  postedDate: string;
  deadline: string;
};

export type CandidateApplication = {
  id: number;
  jobId: number;
  jobSeekerId: number;
  candidateName: string;
  status: string;
  appliedDate: string;
  cvUrl: string;
};

export type SeekerProfile = {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  location: string;
  aboutMe: string;
  gitHubUrl: string;
  linkedInUrl: string;
  portfolioUrl: string;
  cvUrl: string;
  skills: string[];
};

export type AdminStats = {
  totalUsers: number;
  totalCompanies: number;
  totalJobs: number;
  totalApplications: number;
  pendingJobApprovals: number;
};
