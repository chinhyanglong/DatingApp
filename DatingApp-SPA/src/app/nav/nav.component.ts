import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};
  photoUrl: string;

  constructor(public authServices: AuthService, private alertify: AlertifyService,
    private router: Router) { }

  ngOnInit() {
    this.authServices.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
  }

  login() {
    this.authServices.login(this.model).subscribe(next => {
      this.alertify.success('logged in successfully');
    }, error => {
      this.alertify.error('Failed to login');
    }, () => {
      this.router.navigate(['/member']);
    });
  }

  loggedIn() {
   return this.authServices.loggedIn();
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.authServices.decodedToken = null;
    this.authServices.currenUser = null;
    this.alertify.message('logged out');
    this.router.navigate(['/home']);
  }

}
