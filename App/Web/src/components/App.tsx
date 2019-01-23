// source: https://github.com/vikpe/react-webpack-typescript-starter
import * as React from "react";
import { AppBar, Typography, Tabs, Tab, Grid } from "@material-ui/core";
import SideMenu from "./SideMenu";
import ContourList from "./ContourList";
import ProgressSpinner from "./ProgressSpinner";
import DrawManually from "./DrawManually";
import DrawAutomatic from "./DrawAutomatic";
import PatientInfo from "./PatientInfo";
import ContourSelectorList from "./ContourSelectorList";
import PreviewSavedContours from "./PreviewSavedContours";

const gridStyle = {
    margin: "2rem"
};

export interface AppViewProps {
}
export interface AppViewState {
    readonly tab: number;
}

export default class AppView extends React.Component<AppViewProps, AppViewState> {
    constructor(props) {
        super(props);
        this.state = {
            tab: 0
        };
    }
    handleChange = (event, tab) => {
        this.setState({ tab });
        console.warn("tab");
        console.warn(tab);
    }
    render() {
        return (
            <>
                <AppBar
                    position="sticky"
                    color="primary"
                    style={{ display: "inline-block" }}
                >
                    <Typography
                        variant="h5"
                        color="inherit"
                        style={{ margin: "1rem" }}
                    >
                        DICOM contour
                    </Typography>
                </AppBar>
                <div style={{ float: "left", width: "20%" }}>
                    <SideMenu />
                </div>
                <div style={{ float: "left", width: "60%", textAlign: "center" }}>
                    <Tabs
                        fullWidth
                        value={this.state.tab}
                        textColor="primary"
                        onChange={this.handleChange}
                    >
                        <Tab label={"manual"} />
                        <Tab label={"semi-automatic"} />
                        <Tab label={"preview saved"} />
                    </Tabs>
                    <Grid container>
                        <Grid item id="pictute" style={{ ...gridStyle }}>
                            {this.state.tab === 0 && <DrawManually />}
                            {this.state.tab === 1 && <DrawAutomatic />}
                            {this.state.tab === 2 && <PreviewSavedContours />}
                        </Grid>
                        <Grid item id="patient_info" style={{ ...gridStyle }}>
                            <PatientInfo />
                        </Grid>
                    </Grid>
                </div>
                <div style={{ float: "left", width: "20%" }}>
                    {this.state.tab === 0 && <ContourList />}
                    {this.state.tab === 1 && <ContourList />}
                    {this.state.tab === 2 && <ContourSelectorList />}
                </div>
                <ProgressSpinner />
            </>
        );
    }
}

