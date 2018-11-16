// source: https://github.com/vikpe/react-webpack-typescript-starter
import * as React from "react";
// import "./../assets/scss/App.scss";
import CornerstoneElement from "./DicomViewer";
import { AppBar, Typography } from "@material-ui/core";
import FileInputButton from "./FileInput";


export interface AppProps {
}

const imageId =
    "https://rawgit.com/cornerstonejs/cornerstoneWebImageLoader/master/examples/Renal_Cell_Carcinoma.jpg";

const stack = {
    imageIds: [imageId],
    currentImageIdIndex: 0
};

export default class App extends React.Component<AppProps, undefined> {
    handleFiles(files: FileList) {
        for (let i = 0; i < files.length; i++) {
            const file = files[i];
            console.warn(file.name);
        }
    }

    render() {
        return (
            <>
                <AppBar
                    position="sticky"
                    color="primary"
                >
                    <Typography
                        variant="h5"
                        color="inherit"
                        style={{ margin: "1rem" }}
                    >
                        DICOM contour
                    </Typography>
                </AppBar>
                <FileInputButton onFilesSelect={(files: FileList) => this.handleFiles(files)} fileType=".txt" style={{margin: "1rem"}}>
                    INPUT
                </FileInputButton>
                <div style={{ overflow: "auto", height: "100%" }}>
                    <CornerstoneElement stack={{ ...stack }} />
            </div>
            </>
        );
    }
}
