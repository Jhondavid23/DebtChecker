import type { ToastType } from "./ToastProps";

export interface ToastNotification {
  id: string;
  type: ToastType;
  title: string;
  message?: string;
  duration?: number;
}

export interface ToastContainerProps {
  toasts: ToastNotification[];
  onClose: (id: string) => void;
}