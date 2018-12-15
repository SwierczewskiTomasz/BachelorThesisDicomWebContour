import * as React from "react";
import { Drawer, List, ListItem } from "@material-ui/core";
import { connect } from "react-redux";
import { Dispatch } from "redux";
import { fetchSeries } from "../containers/series/reducers";
import { fetchPatients } from "../containers/patients/reducers";
import { fetchStudies } from "../containers/studies/reducers";
import { fetchInstances } from "../containers/instances/reducers";


export interface SideMenuProps {
    readonly patientsIds: string[];
    readonly seriesIds: string[];
    readonly studiesIds: string[];
    readonly fetchSeriesIds: (id: string) => void;
    readonly fetchPatientsIds: () => void;
    readonly fetchStudiesIds: (id: string) => void;
    readonly fetchInstancesIds: (id: string) => void;
}

interface SideMenuState {
    readonly listType: "patients" | "studies" | "series" | "instances";
    readonly patientId?: string;
    readonly studyId?: string;
}


class SideMenuView extends React.Component<SideMenuProps, SideMenuState> {
    constructor(props) {
        super(props);
        this.state = {
            listType: "patients"
        };
    }
    componentDidMount() {
        this.props.fetchPatientsIds();
    }

    render() {

        return (
            <List>
                <ListItem>{this.state.listType}</ListItem>
                {this.state.listType === "patients" && this.props.patientsIds
                    .map(id => <ListItem
                        button
                        key={id}
                        onClick={() => this.setState({ listType: "studies", patientId: id }, () => this.props.fetchStudiesIds(id))}
                    >
                        {id}
                    </ListItem>)}
                {this.state.listType === "studies" && this.props.studiesIds
                    .map(id => <ListItem
                        button
                        key={id}
                        onClick={() => this.setState({ listType: "series", studyId: id }, () => this.props.fetchSeriesIds(id))}
                    >
                        {id}
                    </ListItem>)}
                {this.state.listType === "series" && this.props.seriesIds.map(id => <ListItem
                    onClick={() => this.props.fetchInstancesIds(id)}
                    button key={id}>{id}</ListItem>)}
            </List >
        );
    }
}

export default connect(
    (state: AppState) => {
        return {
            seriesIds: state.seriesIds,
            patientsIds: state.patientsIds,
            studiesIds: state.studiesIds
        };
    },
    (dispatch: Dispatch<any>) => ({
        fetchSeriesIds: (id: string) => {
            dispatch(fetchSeries("studies/" + id + "/series"));
        },
        fetchStudiesIds: (id: string) => {
            dispatch(fetchStudies("patients/" + id + "/studies"));
        },
        fetchInstancesIds: (id: string) => {
            dispatch(fetchInstances("series/" + id + "/instances"));
        },
        fetchPatientsIds: () => {
            dispatch(fetchPatients());
        }
    })
)(SideMenuView);
