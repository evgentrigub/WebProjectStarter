import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable, throwError } from 'rxjs';
import { UserDTO } from '../models/user';
import { CustomErrorResponse } from '../models/custom-error-response';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  constructor(private http: HttpClient) {}

  getAll(): Observable<UserDTO[]> {
    return this.http.get<UserDTO[]>(`${environment.apiUrl}/users/getall`).pipe(catchError(this.handleError));
  }

  getById(id: string): Observable<UserDTO> {
    return this.http.get<UserDTO>(`${environment.apiUrl}/users/${id}`).pipe(catchError(this.handleError));
  }

  update(user: UserDTO): Observable<null> {
    return this.http.put<null>(`${environment.apiUrl}/users/${user.id}`, user).pipe(catchError(this.handleError));
  }

  delete(id: string): Observable<null> {
    return this.http.delete<null>(`${environment.apiUrl}/users/${id}`).pipe(catchError(this.handleError));
  }

  private handleError(error: CustomErrorResponse) {
    const msg = error.message + ` Status Code: ${error.status}`;
    console.error('AuthenticationService::handleError() ' + msg);
    return throwError('Error: ' + msg);
  }
}
