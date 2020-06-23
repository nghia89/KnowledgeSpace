import { Injectable, enableProdMode } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BaseService } from './base.service';
import { catchError, map } from 'rxjs/operators';
import { User, Function, Pagination } from './../../shared/models';
import { environment } from './../../../environments/environment';
import { AuthService, UtilitiesService } from '../services';

@Injectable({ providedIn: 'root' })
export class UserService extends BaseService {
    public httpOptions = {};
    baseRouter: string;
    constructor(
        private http: HttpClient, private utilitiesService: UtilitiesService, private authService: AuthService) {
        super();
        this.httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                // 'Authorization': `${this.authService.authorizationHeaderValue}`
            })
        };
        this.baseRouter = '/api/users/';
    }

    httpPost(entity: any, path: string) {
        return this.http.post(`${environment.apiUrl}${path}`, JSON.stringify(entity), this.httpOptions)
            .pipe(catchError(this.handleError));
    }

    httpPut(id: string, entity: any, path: string) {
        return this.http.put(`${environment.apiUrl}${path}${id}`, JSON.stringify(entity), this.httpOptions)
            .pipe(catchError(this.handleError));
    }

    add(entity: User) {
        return this.httpPost(entity, '/api/users');
    }

    update(id: string, entity: User) {
        return this.httpPut(id, entity, this.baseRouter);
    }

    getDetail(id) {
        return this.http.get<User>(`${environment.apiUrl}/api/users/${id}`, this.httpOptions)
            .pipe(catchError(this.handleError));
    }

    getAllPaging(filter, pageIndex, pageSize) {
        return this.http.get<Pagination<User>>(`${environment.apiUrl}/api/users/filter?pageIndex=${pageIndex}&pageSize=${pageSize}&filter=${filter}`, this.httpOptions)
            .pipe(map((response: Pagination<User>) => {
                return response;
            }), catchError(this.handleError));
    }

    delete(id) {
        return this.http.delete(environment.apiUrl + '/api/users/' + id, this.httpOptions)
            .pipe(
                catchError(this.handleError)
            );
    }

    getUserRoles(userId: string) {
        return this.http.get<string[]>(`${environment.apiUrl}/api/users/${userId}/roles`, this.httpOptions)
            .pipe(catchError(this.handleError));
    }

    removeRolesFromUser(id, roleNames: string[]) {
        let rolesQuery = '';
        for (const roleName of roleNames) {
            rolesQuery += 'roleNames' + '=' + roleName + '&';
        }
        return this.http.delete(environment.apiUrl + '/api/users/' + id + '/roles?' + rolesQuery, this.httpOptions)
            .pipe(
                catchError(this.handleError)
            );
    }

    assignRolesToUser(userId: string, assignRolesToUser: any) {
        return this.http.post(`${environment.apiUrl}/api/users/${userId}/roles`,
            JSON.stringify(assignRolesToUser), this.httpOptions)
            .pipe(catchError(this.handleError));
    }

    getAll() {
        // return this.httpGet(User, '/api/users');
        return this.http.get<User[]>(`${environment.apiUrl}/api/users`, this.httpOptions)
            .pipe(catchError(this.handleError));
    }

    getMenuByUser(userId: string) {
        return this.http.get<Function[]>(`${environment.apiUrl}/api/users/${userId}/menu`, this.httpOptions)
            .pipe(map(response => {
                const functions = this.utilitiesService.UnflatteringForLeftMenu(response);
                return functions;
            }), catchError(this.handleError));
    }
}