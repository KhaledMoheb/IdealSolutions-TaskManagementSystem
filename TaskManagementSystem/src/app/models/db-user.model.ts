export interface DBUser {
    id: number;
    userName: string;
    password: string;
    email: string;
    phoneNumber: string;
    role: 'Admin' | 'User';
  }