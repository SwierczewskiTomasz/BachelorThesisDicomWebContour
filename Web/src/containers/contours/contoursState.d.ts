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
        readonly statistics: {
            readonly centerOfMass: { x: number, y: number };
            readonly histogram: number[];
            readonly histogramMin: number;
            readonly histogramMax: number;
            readonly histogramMean: number;
            readonly area: number;
            readonly permieter: number;
            readonly numberOfPixelsInsideContour: number;
            readonly numberOfPixelsOfContour: number;
        } | undefined;
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
        readonly statistics: {
            readonly centerOfMass: { x: number, y: number };
            readonly histogram: number[];
            readonly histogramMin: number;
            readonly histogramMax: number;
            readonly histogramMean: number;
            readonly area: number;
            readonly permieter: number;
            readonly numberOfPixelsInsideContour: number;
            readonly numberOfPixelsOfContour: number;
        } | undefined;
    },
    readonly preview: any;
    readonly selectedContourGuids: string[];
}

declare interface AppState extends ContoursState { }
