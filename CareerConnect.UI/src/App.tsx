import { type FormEvent, useEffect, useState } from 'react';
import { BrowserRouter, Link, Route, Routes } from 'react-router-dom';
import { applyToJob, createJob, getAdminStats, getApiError, getJobApplications, getJobs, getMyApplications, getMyJobs, getMyProfile, login, register, updateApplicationStatus, updateMyProfile } from './services/api';
import type { AdminStats, ApplicationRecord, AuthResponse, CandidateApplication, Job, RecruiterJob, SeekerProfile } from './types';
import './App.css';

function readSession(): AuthResponse | null {
  try {
    const session = localStorage.getItem('careerconnect_session');
    return session ? JSON.parse(session) as AuthResponse : null;
  } catch {
    return null;
  }
}

function storeSession(session: AuthResponse) {
  localStorage.setItem('careerconnect_token', session.token);
  localStorage.setItem('careerconnect_session', JSON.stringify(session));
}

function HomePage() {
  const [jobs, setJobs] = useState<Job[]>([]);
  const [error, setError] = useState('');

  useEffect(() => {
    getJobs().then(setJobs).catch((reason: unknown) => setError(getApiError(reason)));
  }, []);

  return (
    <>
      <section className="hero-panel">
        <div>
          <div className="eyebrow">CareerConnect</div>
          <h1>Find the next step in your career.</h1>
          <p>
            CareerConnect helps job seekers discover internships, full-time roles,
            and recruitment opportunities through a modern employer dashboard.
          </p>
          <div className="hero-actions">
            <Link to="/jobs" className="primary-btn">Browse jobs</Link>
            <Link to="/register" className="secondary-btn">Create account</Link>
          </div>
        </div>
        <div className="hero-card">
          <p>Current opportunities</p>
          {jobs[0] ? <><h3>{jobs[0].title}</h3><span>{jobs[0].companyName} · {jobs[0].location}</span></> : <h3>{error ? 'Listings unavailable' : 'No listings yet'}</h3>}
        </div>
      </section>

      <section className="content-panel">
        <div className="section-header">
          <h2>Featured roles</h2>
          <Link to="/jobs">View all</Link>
        </div>

        {error ? <p className="error-message">Could not load live jobs: {error}</p> : null}
        {!error && jobs.length === 0 ? <p>No jobs have been posted yet.</p> : null}
        <div className="job-list">
          {jobs.slice(0, 3).map((job) => (
            <article key={job.id} className="job-card">
              <div className="job-meta-row">
                <span className="badge accent">{job.jobType}</span>
                <span className="badge">{job.workMode}</span>
              </div>
              <h3>{job.title}</h3>
              <p className="job-company">{job.companyName}</p>
              <p>{job.description}</p>
              <div className="meta-line">
                <span>{job.location}</span>
                <span>{job.salary}</span>
              </div>
              <div className="chip-row">
                {(job.skills ?? []).slice(0, 3).map((skill) => (
                  <span key={skill}>{skill}</span>
                ))}
              </div>
            </article>
          ))}
        </div>
      </section>
    </>
  );
}

function JobsPage({ session }: { session: AuthResponse | null }) {
  const [jobs, setJobs] = useState<Job[]>([]);
  const [error, setError] = useState('');
  const [notice, setNotice] = useState('');

  useEffect(() => {
    getJobs().then(setJobs).catch((reason: unknown) => setError(getApiError(reason)));
  }, []);

  const apply = async (jobId: number) => {
    setError('');
    setNotice('');
    try {
      const result = await applyToJob(jobId);
      setNotice(result.message ?? 'Application submitted.');
    } catch (reason) {
      setError(getApiError(reason));
    }
  };

  return (
    <section className="content-panel">
      <div className="section-header">
        <h2>Browse Jobs</h2>
        <span>{jobs.length} results</span>
      </div>

      {error ? <p className="error-message">Could not load live jobs: {error}</p> : null}
      {notice ? <p className="success-message">{notice}</p> : null}
      {!error && jobs.length === 0 ? <p>No jobs have been posted yet.</p> : null}
      <div className="job-list">
        {jobs.map((job) => (
          <article key={job.id} className="job-card">
            <div className="job-meta-row">
              <span className="badge accent">{job.jobType}</span>
              <span className="badge">{job.workMode}</span>
            </div>
            <h3>{job.title}</h3>
            <p className="job-company">{job.companyName}</p>
            <p>{job.description}</p>
            <div className="meta-line">
              <span>{job.location}</span>
              <span>{job.salary}</span>
            </div>
            <div className="chip-row">
              {(job.skills ?? []).slice(0, 4).map((skill) => (
                <span key={skill}>{skill}</span>
              ))}
            </div>
            {session?.role === 'JobSeeker' ? <button className="primary-btn" type="button" onClick={() => void apply(job.id)}>Apply</button> : null}
            {!session ? <Link to="/login" className="secondary-btn">Sign in to apply</Link> : null}
          </article>
        ))}
      </div>
    </section>
  );
}

function LoginPage({ onAuthenticated }: { onAuthenticated: (session: AuthResponse) => void }) {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');

  const submit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setError('');
    setMessage('');
    try {
      const response = await login(email, password);
      storeSession(response);
      onAuthenticated(response);
      setMessage(`Logged in as ${response.role} for ${response.email}`);
    } catch (reason) {
      setError(getApiError(reason));
    }
  };

  return (
    <section className="auth-panel">
      <form className="auth-card" onSubmit={submit}>
        <h2>Login</h2>
        <label>
          Email
          <input type="email" required value={email} onChange={(event) => setEmail(event.target.value)} />
        </label>
        <label>
          Password
          <input type="password" required value={password} onChange={(event) => setPassword(event.target.value)} />
        </label>
        <button type="submit" className="primary-btn">Continue</button>
        {message ? <p className="success-message">{message}</p> : null}
        {error ? <p className="error-message">{error}</p> : null}
      </form>
    </section>
  );
}

function RegisterPage({ onAuthenticated }: { onAuthenticated: (session: AuthResponse) => void }) {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [role, setRole] = useState('JobSeeker');
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [companyName, setCompanyName] = useState('');
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');

  const submit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setError('');
    setMessage('');
    try {
      const response = await register({ email, password, role, firstName, lastName, companyName });
      storeSession(response);
      onAuthenticated(response);
      setMessage(`Welcome ${response.role}. Registration completed.`);
    } catch (reason) {
      setError(getApiError(reason));
    }
  };

  return (
    <section className="auth-panel">
      <form className="auth-card" onSubmit={submit}>
        <h2>Register</h2>
        <label>
          Email
          <input type="email" required value={email} onChange={(event) => setEmail(event.target.value)} />
        </label>
        <label>
          First name
          <input required value={firstName} onChange={(event) => setFirstName(event.target.value)} />
        </label>
        <label>
          Last name
          <input required value={lastName} onChange={(event) => setLastName(event.target.value)} />
        </label>
        <label>
          Password
          <input type="password" required minLength={8} value={password} onChange={(event) => setPassword(event.target.value)} />
        </label>
        <label>
          Role
          <select value={role} onChange={(event) => setRole(event.target.value)}>
            <option value="JobSeeker">Job Seeker</option>
            <option value="Recruiter">Recruiter</option>
          </select>
        </label>
        {role === 'Recruiter' ? <label>Company name<input required value={companyName} onChange={(event) => setCompanyName(event.target.value)} /></label> : null}
        <button type="submit" className="primary-btn">Create account</button>
        {message ? <p className="success-message">{message}</p> : null}
        {error ? <p className="error-message">{error}</p> : null}
      </form>
    </section>
  );
}

function CreateJobPage({ session }: { session: AuthResponse | null }) {
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');
  const [form, setForm] = useState({ title: '', description: '', location: '', jobType: 'FullTime', workMode: 'Remote', salary: '', qualifications: '', experienceLevel: '', deadline: '', vacancies: '1', skills: '' });
  const update = (field: keyof typeof form, value: string) => setForm((current) => ({ ...current, [field]: value }));

  const submit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setError('');
    setMessage('');
    try {
      await createJob({ ...form, companyId: 0, vacancies: Number(form.vacancies), deadline: new Date(form.deadline).toISOString(), status: 'Active', skills: form.skills.split(',').map((skill) => skill.trim()).filter(Boolean) });
      setMessage('Job listing saved to the database.');
    } catch (reason) {
      setError(getApiError(reason));
    }
  };

  if (session?.role !== 'Recruiter') return <section className="content-panel"><h2>Recruiter access required</h2><p>Sign in with a recruiter account to publish a listing.</p><Link to="/login" className="primary-btn">Sign in</Link></section>;

  return <section className="auth-panel"><form className="auth-card" onSubmit={submit}>
    <h2>Post a job</h2>
    <label>Job title<input required value={form.title} onChange={(event) => update('title', event.target.value)} /></label>
    <label>Description<textarea required rows={5} value={form.description} onChange={(event) => update('description', event.target.value)} /></label>
    <label>Location<input required value={form.location} onChange={(event) => update('location', event.target.value)} /></label>
    <label>Job type<select value={form.jobType} onChange={(event) => update('jobType', event.target.value)}><option value="FullTime">Full time</option><option value="PartTime">Part time</option><option value="Internship">Internship</option><option value="Contract">Contract</option></select></label>
    <label>Work mode<select value={form.workMode} onChange={(event) => update('workMode', event.target.value)}><option value="Remote">Remote</option><option value="Hybrid">Hybrid</option><option value="OnSite">On-site</option></select></label>
    <label>Salary<input required value={form.salary} onChange={(event) => update('salary', event.target.value)} /></label>
    <label>Qualifications<input required value={form.qualifications} onChange={(event) => update('qualifications', event.target.value)} /></label>
    <label>Experience level<input required value={form.experienceLevel} onChange={(event) => update('experienceLevel', event.target.value)} /></label>
    <label>Skills, separated by commas<input value={form.skills} onChange={(event) => update('skills', event.target.value)} /></label>
    <label>Application deadline<input required type="date" value={form.deadline} onChange={(event) => update('deadline', event.target.value)} /></label>
    <label>Vacancies<input required type="number" min="1" value={form.vacancies} onChange={(event) => update('vacancies', event.target.value)} /></label>
    <button type="submit" className="primary-btn">Publish listing</button>
    {message ? <p className="success-message">{message}</p> : null}
    {error ? <p className="error-message">{error}</p> : null}
  </form></section>;
}

function DashboardPage({ session }: { session: AuthResponse | null }) {
  const [applications, setApplications] = useState<ApplicationRecord[]>([]);
  const [jobs, setJobs] = useState<RecruiterJob[]>([]);
  const [candidates, setCandidates] = useState<CandidateApplication[]>([]);
  const [selectedJobId, setSelectedJobId] = useState<number | null>(null);
  const [error, setError] = useState('');
  const [adminStats, setAdminStats] = useState<AdminStats | null>(null);

  useEffect(() => {
    setError('');
    if (session?.role === 'JobSeeker') {
      getMyApplications().then(setApplications).catch((reason: unknown) => setError(getApiError(reason)));
    } else if (session?.role === 'Recruiter') {
      getMyJobs().then(setJobs).catch((reason: unknown) => setError(getApiError(reason)));
    } else if (session?.role === 'Admin') {
      getAdminStats().then(setAdminStats).catch((reason: unknown) => setError(getApiError(reason)));
    }
  }, [session]);

  const showCandidates = async (jobId: number) => {
    setSelectedJobId(jobId);
    setError('');
    try {
      setCandidates(await getJobApplications(jobId));
    } catch (reason) {
      setError(getApiError(reason));
    }
  };

  const setStatus = async (applicationId: number, status: string) => {
    setError('');
    try {
      await updateApplicationStatus(applicationId, status);
      if (selectedJobId !== null) await showCandidates(selectedJobId);
    } catch (reason) {
      setError(getApiError(reason));
    }
  };

  if (!session) return <section className="content-panel"><h2>Dashboard</h2><p>Sign in to view activity saved to your account.</p><Link to="/login" className="primary-btn">Sign in</Link></section>;

  return <section className="content-panel">
    <div className="section-header"><h2>Dashboard</h2><span>{session.email} · {session.role}</span></div>
    {error ? <p className="error-message">{error}</p> : null}
    {session.role === 'JobSeeker' ? <>
      <h3>My applications</h3>
      {applications.length === 0 ? <p>You have not applied to any jobs yet.</p> : <div className="job-list">{applications.map((application) => <article className="job-card" key={application.id}><h3>{application.jobTitle}</h3><p className="job-company">{application.companyName}</p><p>Status: {application.status}</p><p>Applied {new Date(application.appliedDate).toLocaleDateString()}</p></article>)}</div>}
      <Link to="/jobs" className="primary-btn">Browse jobs</Link>
    </> : null}
    {session.role === 'Recruiter' ? <>
      <div className="section-header"><h3>My job listings</h3><Link to="/jobs/new">Post a job</Link></div>
      {jobs.length === 0 ? <p>You have not posted any jobs yet.</p> : <div className="job-list">{jobs.map((job) => <article className="job-card" key={job.id}><h3>{job.title}</h3><p className="job-company">{job.companyName}</p><p>{job.location} · {job.status}</p><p>Deadline {new Date(job.deadline).toLocaleDateString()}</p><button type="button" className="secondary-btn" onClick={() => void showCandidates(job.id)}>View applicants</button></article>)}</div>}
      {selectedJobId !== null ? <><h3>Applicants</h3>{candidates.length === 0 ? <p>No applications have been submitted for this listing.</p> : <div className="job-list">{candidates.map((candidate) => <article className="job-card" key={candidate.id}><h4>{candidate.candidateName}</h4><p>{candidate.status} · Applied {new Date(candidate.appliedDate).toLocaleDateString()}</p>{candidate.cvUrl ? <a href={candidate.cvUrl} target="_blank" rel="noreferrer">View CV</a> : null}<label className="status-control">Application status<select value={candidate.status} onChange={(event) => void setStatus(candidate.id, event.target.value)}><option>Applied</option><option>UnderReview</option><option>Shortlisted</option><option>Interview</option><option>Selected</option><option>Rejected</option></select></label></article>)}</div>}</> : null}
    </> : null}
    {session.role === 'Admin' && adminStats ? <div className="stats-grid">{[
      ['Users', adminStats.totalUsers],
      ['Companies', adminStats.totalCompanies],
      ['Jobs', adminStats.totalJobs],
      ['Applications', adminStats.totalApplications],
      ['Pending approvals', adminStats.pendingJobApprovals],
    ].map(([label, value]) => <div className="stat-card" key={label}><strong>{value}</strong><span>{label}</span></div>)}</div> : null}
  </section>;
}

function ProfilePage({ session }: { session: AuthResponse | null }) {
  const [profile, setProfile] = useState<SeekerProfile | null>(null);
  const [skillsText, setSkillsText] = useState('');
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');

  useEffect(() => {
    if (session?.role === 'JobSeeker') {
      getMyProfile().then((data) => {
        setProfile(data);
        setSkillsText(data.skills.join(', '));
      }).catch((reason: unknown) => setError(getApiError(reason)));
    }
  }, [session]);

  if (session?.role !== 'JobSeeker') return <section className="content-panel"><h2>Job seeker access required</h2><p>Sign in with a job seeker account to edit your profile.</p><Link to="/login" className="primary-btn">Sign in</Link></section>;

  const update = (field: keyof SeekerProfile, value: string) => {
    setProfile((current) => current ? { ...current, [field]: value } : current);
  };

  const submit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (!profile) return;
    setError('');
    setMessage('');
    try {
      await updateMyProfile({ ...profile, skills: skillsText.split(',').map((skill) => skill.trim()).filter(Boolean) });
      setMessage('Profile changes saved.');
    } catch (reason) {
      setError(getApiError(reason));
    }
  };

  return <section className="auth-panel"><form className="auth-card" onSubmit={submit}>
    <h2>My profile</h2>
    {error ? <p className="error-message">{error}</p> : null}
    {!profile && !error ? <p>Loading profile…</p> : null}
    {profile ? <>
      <p>{profile.email}</p>
      <label>First name<input required value={profile.firstName} onChange={(event) => update('firstName', event.target.value)} /></label>
      <label>Last name<input required value={profile.lastName} onChange={(event) => update('lastName', event.target.value)} /></label>
      <label>Phone<input value={profile.phone} onChange={(event) => update('phone', event.target.value)} /></label>
      <label>Location<input value={profile.location} onChange={(event) => update('location', event.target.value)} /></label>
      <label>About me<textarea rows={4} value={profile.aboutMe} onChange={(event) => update('aboutMe', event.target.value)} /></label>
      <label>Skills, separated by commas<input value={skillsText} onChange={(event) => setSkillsText(event.target.value)} /></label>
      <label>GitHub URL<input type="url" value={profile.gitHubUrl} onChange={(event) => update('gitHubUrl', event.target.value)} /></label>
      <label>LinkedIn URL<input type="url" value={profile.linkedInUrl} onChange={(event) => update('linkedInUrl', event.target.value)} /></label>
      <label>Portfolio URL<input type="url" value={profile.portfolioUrl} onChange={(event) => update('portfolioUrl', event.target.value)} /></label>
      <label>CV URL<input type="url" value={profile.cvUrl} onChange={(event) => update('cvUrl', event.target.value)} /></label>
      <button type="submit" className="primary-btn">Save profile</button>
      {message ? <p className="success-message">{message}</p> : null}
    </> : null}
  </form></section>;
}

function App() {
  const [session, setSession] = useState<AuthResponse | null>(readSession);

  const logout = () => {
    localStorage.removeItem('careerconnect_token');
    localStorage.removeItem('careerconnect_session');
    setSession(null);
  };

  return (
    <BrowserRouter>
      <div className="app-shell">
        <header className="topbar">
          <div className="brand-wrap">
            <Link to="/" className="brand">CareerConnect</Link>
          </div>
          <nav className="nav-links">
            <Link to="/">Home</Link>
            <Link to="/jobs">Jobs</Link>
            {session?.role === 'Recruiter' ? <Link to="/jobs/new">Post a job</Link> : null}
            <Link to="/dashboard">Dashboard</Link>
            {session?.role === 'JobSeeker' ? <Link to="/profile">Profile</Link> : null}
            {session ? <button type="button" className="nav-button" onClick={logout}>Sign out</button> : <><Link to="/login">Login</Link><Link to="/register">Register</Link></>}
          </nav>
        </header>

        <main className="page-content">
          <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/jobs" element={<JobsPage session={session} />} />
            <Route path="/jobs/new" element={<CreateJobPage session={session} />} />
            <Route path="/login" element={<LoginPage onAuthenticated={setSession} />} />
            <Route path="/register" element={<RegisterPage onAuthenticated={setSession} />} />
            <Route path="/dashboard" element={<DashboardPage session={session} />} />
            <Route path="/profile" element={<ProfilePage session={session} />} />
          </Routes>
        </main>
      </div>
    </BrowserRouter>
  );
}

export default App;
