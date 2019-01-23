declare interface SeriesState {
    readonly series: {
        readonly id: string;
        readonly name: string;
    }[];
}

declare interface AppState extends SeriesState { }
