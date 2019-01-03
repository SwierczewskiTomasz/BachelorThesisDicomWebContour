declare interface StudiesState {
    readonly studies: {
        readonly id: string;
        readonly name: string;
    }[];
    readonly institutionName?: string;
    readonly referringPhysicianName?: string;
    readonly studyDate?: string;
    readonly studyDescription?: string;
}

declare interface AppState extends StudiesState { }
