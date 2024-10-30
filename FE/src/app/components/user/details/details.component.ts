import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { User } from '../../../interfaces/User';
import { UserService } from '../../../services/User/user.service';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, NavigationStart } from '@angular/router';
import { JWTTokenService } from '../../../services/JWTToken/jwttoken.service';
import { UserRole } from '../../../enums/user-roles.enum';
import { DeviceShow } from '../../../interfaces/DeviceShow';
import { Observable } from 'rxjs';
import { UtilityService } from '../../../services/Utility/utility.service';
import { MessageService } from 'primeng/api';
import { DeviceService } from '../../../services/Device/device.service';
import { Router } from '@angular/router';
import { UserShow } from '../../../interfaces/UserShow';

@Component({
  selector: 'user-details',
  templateUrl: './details.component.html',
  styleUrls: ['./details.component.scss'],
})
export class UserDetailsComponent implements OnInit {
  userId: number | null = null;
  user: User | null = null;
  currentUser: User | null = null;
  userIdStr: string | undefined = '';
  showForm: boolean = false;
  isAdmin:boolean = false;
  isCurrentAdmin:boolean = false;
  userRole:string='';
  currentUserRole?: string ;
  devices:DeviceShow[]=[];
  details: string[]=[];
  @ViewChild('updateForm') updateForm!: NgForm;
  usersMap: Map<number, User> = new Map(); 
  imageUrl:string= 'https://st3.depositphotos.com/9998432/13335/v/450/depositphotos_133352010-stock-illustration-default-placeholder-man-and-woman.jpg';

  userRoleNames = UserRole;

  constructor(
    private userService: UserService,
    private deviceService : DeviceService,
    private route: ActivatedRoute,
    private jwtService: JWTTokenService,
    private cdr: ChangeDetectorRef,
    private utilityService: UtilityService,
    private messageService: MessageService,
    private router:Router
  ) {}

  ngOnInit(): void {
    this.isAdmin = this.utilityService.checkIfAdmin();
    if(!this.isAdmin){
      this.router.navigate(['/home']);
    }
    this.route.params.subscribe(params => {
      this.userId = +params['id'];
      this.fetchUserId();
      this.loadUserDetails();
      this.checkIfAdmin();
      this.checkIfCurrentAdmin();
    });
    if (event instanceof NavigationStart) {
      this.messageService.clear();
    }
  }

  showDevies = false;

  toggleDevices() {
    this.showDevies = !this.showDevies;
  }

  toggleForm(): void {
    this.showForm = !this.showForm;
  }

  loadUserDetails(): void {
    if (this.userId !== null) {
      this.userService.getUserById(this.userId).subscribe(user => {
        this.user = user;
        this.userRole = UserRole[this.user.role];
        if (user.country) {
          this.details.push(user.email, user.country, this.userRole);
        }
      },
      error => {
      });
  
      this.deviceService.getDevicesByUserId(this.userId).subscribe(devices => {
        this.devices = devices;
      },
      error => {
      });
    }
  }
  

  fetchUserId(): void {
    this.userIdStr = this.jwtService.getUserId();
  }

  updateUser(formValue: any): void {
    if (this.isAdmin && this.user && this.userIdStr) {
      const updatedUser: UserShow = {
        id: this.user.id,
        firstName: formValue.firstName || this.user.firstName,
        lastName: formValue.lastName || this.user.lastName,
        country: formValue.country || this.user.country,
      };
  
      this.userService.updateUser(this.user.id, updatedUser).subscribe(
        () => {
          this.showToast("success", "User updated successfully!");
          setTimeout(() => window.location.reload(), 1000);
        },
        error => {
          console.error("Update failed", error);
          this.showToast("error", "User update failed.");
        }
      );
    }
  }
  
  
  
  checkIfAdmin(): void {
    const role = this.jwtService.getUserRole();
    this.isAdmin = this.utilityService.checkIfAdmin();
  }

  checkIfCurrentAdmin(): void {
    if (this.userId) {
        this.userService.getUserById(this.userId).subscribe(
            user => {
                this.currentUser = user;
                this.currentUserRole = user.role.toString();
                this.isCurrentAdmin = this.currentUserRole == "Admin";

            },
            error => {
                console.error("Error fetching user details:", error);
            }
        );
    } else {
        console.warn("User ID is null or undefined.");
    }
}


  


  inactivateUser(): void {
    if (this.userId !== null && this.isAdmin && this.user?.role.toString().toUpperCase()==UserRole[0]) {
      this.userService.inactivateUser(this.userId, 1).subscribe(
        () => { this.showToast("success", "inactivating user worked!") },
        error => {  this.showToast("error", "inactivating user didn't work!") }
      );
    }
    console.log(this.user?.role.toString().toUpperCase())
    console.log(UserRole[0])
  }
  showToast(severity: string, summary: string) {
    this.messageService.add({ severity, summary, life: 3000 });
  }
  activateUser(): void {
    if (this.userId !== null && this.isAdmin) {
      this.userService.inactivateUser(this.userId, 0).subscribe(
        () => { this.showToast("success", "activating user worked!")},
        error => {this.showToast("error", "activating user didn't work!") }
      );
    }
  }

  changeRole(role: number): void {
    if (this.userId !== null && this.isAdmin ) {
      this.userService.changeRole(this.userId, role).subscribe(
        () => { this.showToast("success", "changing the user role worked!")},
        error => {
          this.showToast("error", "changing the role of the user didn't work!")
        }
      );
    }
  }

  getUserName(userId: number): string {
    return this.usersMap.get(userId)?.firstName || 'Unknown';
  }
}
