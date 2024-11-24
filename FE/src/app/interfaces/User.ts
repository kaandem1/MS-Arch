import { UserRole } from "../enums/user-roles.enum"
import { UserShow } from "./UserShow"


export interface User extends UserShow{
    role:UserRole;
    email:string;
}