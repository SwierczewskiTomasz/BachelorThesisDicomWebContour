declare interface PatientsState {
    readonly patientsIds: string[];
}

declare interface AppState extends PatientsState { }
