import { BrowserRouter, Routes, Route, Navigate, useNavigate } from 'react-router-dom'
import Login from './pages/Login'
import Conversations from './pages/Conversations'
import ConversationDetail from './pages/ConversationDetail'
import { loadAuth } from './auth/tokenStore'
import RegisterUser from './pages/RegisterUser'
import ChannelSettings from './pages/ChannelSettings';
import Statistics from './pages/Statistics';

function PrivateRoute({ children }: { children: JSX.Element }) {
  const { accessToken } = loadAuth()
  return accessToken ? children : <Navigate to="/login" replace />
}

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/" element={<PrivateRoute><Conversations /></PrivateRoute>} />
        <Route path="/conversation" element={<PrivateRoute><ConversationDetail /></PrivateRoute>} />
        <Route path="*" element={<Navigate to="/" replace />} />
	<Route path="/users/new" element={<PrivateRoute><RegisterUser /></PrivateRoute>} />
	<Route path="/channels" element={<PrivateRoute><ChannelSettings /></PrivateRoute>} />
        <Route path="/statistics" element={<PrivateRoute><Statistics /></PrivateRoute>} />

      </Routes>
    </BrowserRouter>
  )
}
