﻿import { Api } from './api';
import { JoinStatus } from '../viewmodels/boardRow';
import { UserJoin } from '../viewmodels/userJoin';
import { inject } from 'aurelia-framework';
import { ErrorParser } from './errorParser';
import { HttpClient, json } from 'aurelia-fetch-client';
import * as  Uri from 'urijs';
import { Board } from '../services/models/board';

@inject(HttpClient, ErrorParser)
export class BoardService extends Api {

    constructor(
        http: HttpClient,
        private readonly errorParser: ErrorParser) {

        super(http);
    }

    public async findAll(creator: string = '', member: string = ''): Promise<Board[]> {

        let uri = Uri('api/boards');
        if (creator) {
            uri.addSearch('creator', creator);
        }
        if (member) {
            uri.addSearch('member', member);
        }

        return await super.get(uri.valueOf());
    }

    public async find(id: number): Promise<Board> {

        let url = `api/boards/${id}`;
        return await super.get(url);
    }

    public async getWithUsers(id: number) {

        let url = `api/boards/${id}?fields=id,name,users,isPrivate`;
        return await super.get(url);
    }

    public add(board: Board) {

        return this.post('api/boards', board);
    }

    public edit(board: any) {

        return this.put('api/boards', board);
    }

    public async remove(id: number) {

        let url = `api/boards/${id}`;
        return await this.deleteCall(url);
    }

    public async join(boardId: number) {

        return await this.post('api/joins', boardId);
    }

    public async getJoinRequests(userId: string | undefined) {

        let url = `api/joins?candidateId=${userId}`;
        let data = await super.get(url);
        return this.toUserJoins(data, userId);
    }

    private toUserJoins(data: any, userId: string | undefined) {

        let userJoins: UserJoin[] = [];

        for (let i in data) {

            let joinRequest = data[i];
            userJoins.push(new UserJoin(joinRequest.boardId, joinRequest.status));
        }

        return userJoins;
    }

    public async getJoinRequestsForBoard(boardId: number) {

        let url: string = `api/boards/${boardId}/joins?status=waiting`;
        return await super.get(url);
    }

    public acceptJoin(joinId: number) {

        return this.sendDecision(joinId, true);
    }

    public rejectJoin(joinId: number) {

        return this.sendDecision(joinId, false);
    }

    private async sendDecision(joinId: number, isAccepted: boolean) {

        return await this.post('api/joins/decision', { joinRequestId: joinId, isAccepted: isAccepted });
    }

    public async setIsPrivate(boardId: number, isPrivate: boolean) {

        //let operations = [
        //    { "op": "replace", "path": "/isPrivate", value: isPrivate }
        //];
        let request = {
            method: 'PATCH',
            body: json([this.getReplacePatchOperation('/isPrivate', isPrivate)])
        };

        await this.http.fetch(`api/boards/${boardId}`, request);
    }

    public async setKudoAcceptanceType(boardId: number, acceptanceType: number) {

        let request = {
            method: 'PATCH',
            body: json([this.getReplacePatchOperation('/acceptanceType', acceptanceType)])
        };

        await this.http.fetch(`api/boards/${boardId}`, request);
    }

    private getReplacePatchOperation(path: string, value: any) {
        return { "op": "replace", "path": path, value: value };
    }

    public async inviteUsers(boardId: number, emails: string[]) {
        console.log(emails);

        let emailsRequest = emails.map(x => { return { email: x }; });
        let url = `api/boards/${boardId}/invitations`;
        await this.post(url, emailsRequest);
    }

    public async acceptInvitation(boardId: number, code: string) {

        let request = { code: code };
        let url = `api/boards/${boardId}/invitations/approval`;

        console.log(url, 'service');

        return await this.post(url, code);
    }
}