import { HttpClient, HttpParams } from "@angular/common/http";
import { PaginatedResult } from "../_model/pagination";
import { map } from "rxjs";

//extracted pagination functons to seperate helper file so that it could be used across entire context and seperate from member service: seperation of concerns


//This method is resposible for making HTTP requests and extracting the paginated results
export function getPaginatedResults<T>(url: string, params: HttpParams, http: HttpClient) {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>

    return http.get<T>(url, { observe: 'response', params }).pipe( //makes a get request to API endpoint with the parameters of the query
        map(response => {
            if (response.body) {
                paginatedResult.result = response.body; //recieve the result of that query which should be a subset of the results that match params
            }
            const pagination = response.headers.get('Pagination'); //extracts the pagination details from the header of the response
            if (pagination) {
                paginatedResult.pagination = JSON.parse(pagination); //passes the pgination details of response to the user aswell
            }
            return paginatedResult;
        })
    );
}

//This method is responsible for generating the query parameters for pagination in the HTTP request. takes the params from the client
export function getPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams(); //allows you to add queries to a http request since its a query we are sending to specify the amount of users we need from the DB

    params = params.append('pageNumber', pageNumber); //parameter passed too http request
    params = params.append('pageSize', pageSize); //paramter passed to http request
    return params;
}