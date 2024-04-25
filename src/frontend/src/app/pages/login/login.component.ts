import { Component } from '@angular/core';
import { ConfigurationService } from '../../services/configuration.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html'
})
export class LoginPage {
  apiBaseUrl = "";

  constructor(private configService: ConfigurationService) {
    this.apiBaseUrl = this.configService.getApiBaseUrl();
  }
    
  loginWithGithub(): void {
    window.location.href = this.apiBaseUrl + '/api/auth/github-auth';
  }
}