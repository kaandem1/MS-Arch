import { PersonReference } from "./PersonReference"

export interface Device {
    id: number;
    deviceName: string;
    description: string;
    address: string;
    maxHourlyCons: number;
    user: PersonReference;
  }
  