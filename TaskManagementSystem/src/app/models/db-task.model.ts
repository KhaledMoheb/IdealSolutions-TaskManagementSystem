import { DBUser } from "./db-user.model";

export interface DBTask {
    id: number;
    title: string;
    description: string;
    status: 'pending' | 'in-progress' | 'completed'; // Enum-like type for task status
    assignedUserId: number;
    assignedUser: DBUser; // Link to the DBUser
  }