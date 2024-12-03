export interface DeviceConsumption {
    deviceId: number;
    maxHourlyCons: number;
    hourlyConsumption: {
      [timestamp: number]: number;
    };
  }
  