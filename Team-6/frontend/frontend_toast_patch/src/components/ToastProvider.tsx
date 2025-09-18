
import React, { createContext, useCallback, useContext, useMemo, useState } from 'react';

export type ToastType = 'success' | 'error' | 'info' | 'warning';

export interface Toast {
  id: string;
  title?: string;
  message: string;
  type?: ToastType;
  duration?: number; // ms
}

interface ToastContextValue {
  showToast: (toast: Omit<Toast, 'id'>) => void;
  hideToast: (id: string) => void;
}

const ToastContext = createContext<ToastContextValue | undefined>(undefined);

export function useToast() {
  const ctx = useContext(ToastContext);
  if (!ctx) {
    throw new Error('useToast must be used within <ToastProvider />');
  }
  return ctx;
}

export const ToastProvider: React.FC<React.PropsWithChildren> = ({ children }) => {
  const [toasts, setToasts] = useState<Toast[]>([]);

  const hideToast = useCallback((id: string) => {
    setToasts(prev => prev.filter(t => t.id != id));
  }, []);

  const showToast = useCallback((toast: Omit<Toast, 'id'>) => {
    const id = Math.random().toString(36).slice(2, 9);
    const t: Toast = { id, duration: 3000, type: 'info', ...toast };
    setToasts(prev => [...prev, t]);
    // auto-dismiss
    window.setTimeout(() => hideToast(id), t.duration);
  }, [hideToast]);

  const value = useMemo(() => ({ showToast, hideToast }), [showToast, hideToast]);

  return (
    <ToastContext.Provider value={value}>
      {children}
      <ToastShelf toasts={toasts} onClose={hideToast} />
    </ToastContext.Provider>
  );
};

// --- UI ---
const typeStyles: Record<ToastType, string> = {
  success: 'border-green-500 bg-green-50 text-green-900',
  error: 'border-red-500 bg-red-50 text-red-900',
  info: 'border-blue-500 bg-blue-50 text-blue-900',
  warning: 'border-yellow-500 bg-yellow-50 text-yellow-900',
};

const typeDot: Record<ToastType, string> = {
  success: 'bg-green-500',
  error: 'bg-red-500',
  info: 'bg-blue-500',
  warning: 'bg-yellow-500',
};

const CloseIcon: React.FC<React.SVGProps<SVGSVGElement>> = (props) => (
  <svg viewBox="0 0 24 24" width="20" height="20" aria-hidden="true" {...props}>
    <path d="M6.4 6.4l11.2 11.2M17.6 6.4L6.4 17.6" stroke="currentColor" strokeWidth="2" strokeLinecap="round"/>
  </svg>
);

const ToastShelf: React.FC<{ toasts: Toast[]; onClose: (id: string)=>void }> = ({ toasts, onClose }) => {
  return (
    <div className="fixed right-4 top-4 z-50 flex w-[22rem] flex-col gap-3">
      {toasts.map((t) => (
        <ToastCard key={t.id} toast={t} onClose={() => onClose(t.id)} />
      ))}
    </div>
  );
};

const ToastCard: React.FC<{ toast: Toast; onClose: ()=>void }> = ({ toast, onClose }) => {
  const { type = 'info', title, message } = toast;
  return (
    <div
      className={[
        "transform transition-all duration-300",
        "translate-x-0 opacity-100",
        "rounded-2xl border shadow-lg",
        "p-4 pr-3",
        "flex items-start gap-3",
        typeStyles[type],
      ].join(' ')}
      role="status"
    >
      <span className={`mt-1 h-2 w-2 flex-none rounded-full ${typeDot[type]}`} />
      <div className="min-w-0 flex-1">
        {title && <div className="font-semibold leading-tight">{title}</div>}
        <div className="text-sm leading-snug break-words">{message}</div>
      </div>
      <button
        onClick={onClose}
        className="rounded-md p-1/2 text-inherit/70 hover:text-inherit focus:outline-none"
        aria-label="Закрыть уведомление"
        title="Закрыть"
      >
        <CloseIcon />
      </button>
    </div>
  );
};
