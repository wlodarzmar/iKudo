export class MyKudo {

	constructor(){
		this.kudoTypes = [MyKudoSearchOptions.All, MyKudoSearchOptions.Received, MyKudoSearchOptions.Sended];
		this.selectedKudoType = MyKudoSearchOptions.All;
	}

	public kudoTypes: MyKudoSearchOptions[];
	public selectedKudoType: MyKudoSearchOptions;

	public submit() {
		console.log(this.selectedKudoType);
	}

	public refreshSearch() {
		this.selectedKudoType = MyKudoSearchOptions.All;
	}
}

enum MyKudoSearchOptions {

	All = "Wszystkie",
	Received = "Otrzymane",
	Sended = "Wysłane"
}