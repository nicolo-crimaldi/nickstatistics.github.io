export class Server{
    subIntervalNumber;
    lambda;
    attackList [this.subIntervalNumber] = [];

    constructor(subIntervalNumber, lambda){
        this.setAttack(subIntervalNumber, lambda);
    }

    setAttack(subIntervalNumber, lambda){
        this.lambda = lambda;
        this.subIntervalNumber = subIntervalNumber;
        const p = lambda / subIntervalNumber;
        for(i = 0; i < this.attackList.length; i++){
            if(Math.random() > p){
                this.attackList[i] = 1;
            } else{
                this.attackList[i] = 0;
            }
        }
    }

}