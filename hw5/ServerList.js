import { Server } from "./Server";

export class ServerList{
    serverList = [];
    
    constructor(subIntervalNumbers, lambda, serverNumber){
        this.setServerList(subIntervalNumbers, lambda, serverNumber);
    }

    setServerList(subIntervalNumber, lambda, serverNumber){
        for (let i = 0; i < serverNumber; i++) {
            const server = new Server(subIntervalNumber, lambda);
            this.serverList.push(server);
        }
    }

}