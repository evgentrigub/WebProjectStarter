import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { environment } from 'src/environments/environment';
import { map, catchError } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { UserAuthModel, UserDTO } from '../models/user';
import { CustomErrorResponse } from '../models/custom-error-response';
import { Result } from '../models/result-model';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationService {
  private authUrl = `${environment.apiUrl}/users/`;
  private currentUserSubject: BehaviorSubject<UserAuthModel>;

  public currentUser: Observable<UserAuthModel>;

  constructor(private http: HttpClient) {
    this.currentUserSubject = new BehaviorSubject<UserAuthModel>(JSON.parse(localStorage.getItem('currentUser')));
    this.currentUser = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): UserAuthModel {
    return this.currentUserSubject.value;
  }

  /**
   * Send user login and password
   * and return authenticate user
   * @param email user login
   * @param password user password
   */
  login(email: string, password: string): Observable<Result<UserAuthModel>> {
    return this.http.post<Result<UserAuthModel>>(this.authUrl + `authenticate`, { email, password }).pipe(
      map(result => {
        if (result.isSuccess && result.data.token) {
          localStorage.setItem('currentUser', JSON.stringify(result.data));
          this.currentUserSubject.next(result.data);
        }
        return result;
      }),
      catchError(this.handleError)
    );
  }

  /**
   * Send user model for registration
   * and return authnticated user
   * @param user user model
   */
  register(user: UserDTO): Observable<Result<UserAuthModel>> {
    return this.http.post<Result<UserAuthModel>>(this.authUrl + `register`, user).pipe(catchError(this.handleError));
  }

  logout() {
    localStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
  }

  private handleError(error: CustomErrorResponse) {
    const msg = error.message + ` Status Code: ${error.status}`;
    console.error('AuthenticationService::handleError() ' + msg);
    return throwError('Error: ' + msg);
  }
}
