declare interface PatientsState {
    readonly patients: {
        readonly id: string;
        readonly name: string;
    }[];
    readonly name: string;
    readonly birthdate: string;
    readonly sex: string;
    readonly patientId: string;
}

declare interface AppState extends PatientsState { }
