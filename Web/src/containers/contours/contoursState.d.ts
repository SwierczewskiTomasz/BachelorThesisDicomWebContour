declare interface ContoursState {
    readonly contours: {
        readonly guid: string;
        readonly dicomid: string;
        readonly tag: string;
        readonly lines: {
            x: number,
            y: number
        }[];
        readonly width: number;
        readonly height: number;
    }[];
}

declare interface AppState extends ContoursState { }
