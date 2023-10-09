export interface Pagination {
    currentPage: number;
    itemsPerPage: number;
    totalItems: number;
    totalPages: number;
}

export class PaginatedResult<T>{
    result?: T; // result is undefined because its up to user what amount of info they want displayed on the page    
    pagination?: Pagination; //undefiend becuase its up to the user what pagination details they want. this will be poupated with te pagination info
}