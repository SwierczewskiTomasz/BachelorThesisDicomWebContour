import * as React from "react";
import { Drawer, List, ListItem } from "@material-ui/core";
import { connect } from "react-redux";
import { Dispatch } from "redux";
import { fetchSeries, Serie } from "../containers/series/reducers";
import { fetchPatients, Patient } from "../containers/patients/reducers";
import { fetchStudies, Study } from "../containers/studies/reducers";
import { fetchInstances } from "../containers/instances/reducers";


export interface SideMenuProps {
    readonly patients: Patient[];
    readonly series: Serie[];
    readonly studies: Study[];
    readonly fetchSeries: (id: string) => void;
    readonly fetchPatients: () => void;
    readonly fetchStudies: (id: string) => void;
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
        this.props.fetchPatients();
    }

    render() {

        return (
            <List>
                <ListItem>{this.state.listType}</ListItem>
                {this.state.listType === "patients" && this.props.patients
                    .map(p => <ListItem
                        button
                        key={p.id}
                        onClick={() => this.setState({ listType: "studies", patientId: p.id }, () => this.props.fetchStudies(p.id))}
                    >
                        {p.name}
                    </ListItem>)}
                {this.state.listType === "studies" && this.props.studies
                    .map(s => <ListItem
                        button
                        key={s.id}
                        onClick={() => this.setState({ listType: "series", studyId: s.id }, () => this.props.fetchSeries(s.id))}
                    >
                        {(s.name !== undefined && s.name.length > 0) ? s.name : "No name"}
                    </ListItem>)}
                {this.state.listType === "series" && this.props.series.map(s => <ListItem
                    onClick={() => this.props.fetchInstancesIds(s.id)}
                    button key={s.id}>
                    {(s.name !== undefined && s.name.length > 0) ? s.name : "No name"}
                </ListItem>)}
            </List >
        );
    }
}

export default connect(
    (state: AppState) => {
        return {
            series: state.series,
            patients: state.patients,
            studies: state.studies
        };
    },
    (dispatch: Dispatch<any>) => ({
        fetchSeries: (id: string) => {
            dispatch(fetchSeries("studies/" + id + "/series"));
        },
        fetchStudies: (id: string) => {
            dispatch(fetchStudies("patients/" + id + "/studies"));
        },
        fetchInstancesIds: (id: string) => {
            dispatch(fetchInstances("series/" + id + "/instances"));
        },
        fetchPatients: () => {
            dispatch(fetchPatients());
        }
    })
)(SideMenuView);
