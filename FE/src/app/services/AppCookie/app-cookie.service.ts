import { Injectable } from '@angular/core';
import { CookieStore } from '../../interfaces/CookieStore';

@Injectable({
  providedIn: 'root',
})
export class AppCookieService {
  private cookieStore: CookieStore = {};

  constructor() {
    this.parseCookies(document.cookie);
  }

  private parseCookies(cookies = document.cookie) {
    this.cookieStore = {};
    if (!cookies) { return; }
    const cookiesArr = cookies.split(';');
    for (const cookie of cookiesArr) {
      const [key, value] = cookie.split('=');
      this.cookieStore[key.trim()] = value.trim();
    }
  }

  get(key: string): string | null {
    this.parseCookies();
    return this.cookieStore[key] || null;
  }

  set(key: string, value: string) {
    document.cookie = `${key}=${value}; path=/`;
  }

  remove(key: string) {
    document.cookie = `${key}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;`;
  }
}
