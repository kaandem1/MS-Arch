import { UserRole } from "../enums/user-roles.enum";

export interface DecodedToken {
  nameid: string;
  name: string;
  email: string;
  role: UserRole;
  nbf: number;
  exp: number;
  iat: number;
  iss: string;
  aud: string;
}
