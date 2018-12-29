declare interface StudiesState {
    readonly studies: {
        readonly id: string;
        readonly name: string;
    }[];
}

declare interface AppState extends StudiesState { }
