declare interface ContoursState {
    readonly contours: {
        readonly guid: string;
        readonly dicomid: string;
        readonly tag: string;
        readonly lines: {
            readonly points: {
                readonly x: number;
                readonly y: number;
            }[];
            readonly brushColor: string;
        }[];
        readonly width: number;
        readonly height: number;
    }[];
    readonly selectedContour?: {
        readonly guid: string;
        readonly dicomid: string;
        tag: string;
        readonly lines: {
            readonly points: {
                readonly x: number;
                readonly y: number;
            }[];
            readonly brushColor: string;
        }[];
        readonly width: number;
        readonly height: number;
    }
}

declare interface AppState extends ContoursState { }
