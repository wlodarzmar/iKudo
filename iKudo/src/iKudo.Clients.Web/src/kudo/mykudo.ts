import { inject } from 'aurelia-framework';
//import { InputsHelper } from '../inputsHelper';
import { KudoService } from '../services/kudoService';
//import { User } from '../viewmodels/user';
//import { KudoType } from '../viewmodels/kudoType';
//import { Notifier } from '../helpers/Notifier';
//import { Kudo } from '../viewmodels/kudo';

@inject(KudoService)
export class MyKudo {

	constructor(kudoService: KudoService){

		this.kudoService = kudoService;
		this.kudoTypes = [MyKudoSearchOptions.All, MyKudoSearchOptions.Received, MyKudoSearchOptions.Sended];
		this.selectedKudoType = MyKudoSearchOptions.All;
	}

	public kudoTypes: MyKudoSearchOptions[];
	public selectedKudoType: MyKudoSearchOptions;

	private kudoService: KudoService;

	public submit() {
		//this.kudoService.getKudos()	
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