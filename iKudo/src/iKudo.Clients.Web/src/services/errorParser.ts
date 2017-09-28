export class ErrorParser {

    public parse(error: any): string {
        let errorMessage = '';
        for (let i in error) {
            errorMessage += error[i];
        }

        return errorMessage;
    }
}