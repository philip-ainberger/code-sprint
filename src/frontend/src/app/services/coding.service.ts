import { Injectable } from "@angular/core";
import { CodingGrpcServiceClient } from "../generated/Protos/coding.client";
import { ListSprintsRequest, ListSprintsResponse } from "../generated/Protos/coding";
import { StateService } from "./state.service";

export interface ICodingService {
    GetSprints(request: ListSprintsRequest): Promise<ListSprintsResponse>;
}

@Injectable({
    providedIn: 'root'
})
export class CodingService implements ICodingService {

    constructor(private service: CodingGrpcServiceClient, private stateService: StateService) {

    }

    // POC - TODO finisch implementation

    async GetSprints(request: ListSprintsRequest): Promise<ListSprintsResponse> {
        try {
            const res = await this.service.listSprints(request);
            this.stateService.setState("Received sprints");
            return res.response;

        } catch (error) {
            console.error('Failed to get sprints:', error);
            throw error; // Rethrow or handle error as needed
        }
    }
}