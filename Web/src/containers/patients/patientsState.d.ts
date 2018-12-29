declare interface PatientsState {
    readonly patients: {
        readonly id: string;
        readonly name: string;
    }[];
}

declare interface AppState extends PatientsState { }
