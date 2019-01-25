import * as React from "react";
import { Drawer, List, ListItem, ListItemIcon, Divider, ListItemSecondaryAction } from "@material-ui/core";
import { connect } from "react-redux";
import { Dispatch } from "redux";
import PersonIcon from "@material-ui/icons/PersonOutlined";
import { Contour, setCurrentContour as setCurrentContour, discardCurrentContour as discardCurrentContour } from "../containers/contours/reducers";
import ContourWithStatistics from "./ContourWithStatistics";
import { orthancURL } from "../helpers/requestHelper";
import { Size } from "./DrawAutomatic";

export interface ContourListProps {
    readonly contours: Contour[];
    readonly selectedContour?: Contour;
    readonly setCurrentContour: (guid: string) => void;
    readonly discardCurrentContour: () => void;
}

interface ContourListState {
    readonly imgSize: Size;
    readonly size: Size;
}


class ContourListView extends React.Component<ContourListProps, ContourListState> {
    componentDidMount() {
        this.calculateSize(this.props);
    }

    componentWillReceiveProps(props: ContourListProps) {
        this.calculateSize(props);
    }

    calculateSize(props: ContourListProps) {
        if (props.selectedContour !== undefined && props.selectedContour !== null) {
            let img = new Image();
            const fun = (w, h) => {
                this.setState({ imgSize: { width: w, height: h } });
                h = h * 1000 / w;
                w = 1000;
                if (h > 600) {
                    w = w * 600 / h;
                    h = 600;
                }
                this.setState({
                    size: { width: w, height: h }
                });
            };
            img.onload = function () {
                // console.warn(img.naturalWidth, img.naturalHeight);
                fun(img.naturalWidth, img.naturalHeight);
            };
            img.src = orthancURL + "instances/" + props.selectedContour.dicomid + "/preview";
        }
    }



    constructor(props) {
        super(props);
        this.state = {
            imgSize: { width: -1, height: -1 },
            size: { width: -1, height: -1 }
        };
    }

    getSize(width: number, height: number): Size {
        let w = width;
        let h = height;
        h = h * 1000 / w;
        w = 1000;
        if (h > 600) {
            w = w * 600 / h;
            h = 600;
        }
        return { width: w, height: h };
    }

    render() {

        return (
            <>
                <List>
                    <ListItem>
                        {"Available Contours"}
                    </ListItem>
                    <Divider />
                    {this.props.contours
                        .map(c => <ListItem
                            button
                            key={c.guid}
                            onClick={() => this.props.setCurrentContour(c.guid)}
                        >
                            {c.tag}
                            <ListItemSecondaryAction>
                                <ListItemIcon>
                                    <PersonIcon />
                                </ListItemIcon>
                            </ListItemSecondaryAction>
                        </ListItem>)}
                </List>
                <ContourWithStatistics
                    open={this.props.selectedContour !== undefined}
                    contour={this.props.selectedContour}
                    size={this.props.selectedContour ?
                        this.getSize(this.props.selectedContour.width, this.props.selectedContour.height) :
                        { width: 1, height: 1 }}
                    imgUrl={"url(" + (this.props.selectedContour ? orthancURL + "instances/" +
                        this.props.selectedContour.dicomid
                        + "/preview" : "https://imgur.com/t8wK1PH.png") + ")"}
                    onClose={() => this.props.discardCurrentContour()}
                />
            </>
        );
    }
}

export default connect(
    (state: AppState) => {
        return {
            contours: state.contours,
            selectedContour: state.selectedContour
        };
    },
    (dispatch: Dispatch<any>) => ({
        setCurrentContour: (guid: string) => {
            dispatch(setCurrentContour(guid));
        },
        discardCurrentContour: () => {
            dispatch(discardCurrentContour());
        }
    })
)(ContourListView);
