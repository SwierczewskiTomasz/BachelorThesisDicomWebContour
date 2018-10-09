import * as React from "react";
import { render } from "react-dom";
import * as cornerstone from "cornerstone-core";
import * as cornerstoneTools from "cornerstone-tools";
import * as cornerstoneMath from "cornerstone-math";
import * as cornerstoneWebImageLoader from "cornerstone-web-image-loader";

cornerstoneTools.external.cornerstone = cornerstone;
cornerstoneTools.external.cornerstoneMath = cornerstoneMath;
cornerstoneWebImageLoader.external.cornerstone = cornerstone;

const imageId =
    "https://rawgit.com/cornerstonejs/cornerstoneWebImageLoader/master/examples/Renal_Cell_Carcinoma.jpg";

const divStyle: React.CSSProperties = {
    width: "512px",
    height: "512px",
    position: "relative",
    color: "white"
};

const bottomLeftStyle: React.CSSProperties = {
    bottom: "5px",
    left: "5px",
    position: "absolute",
    color: "white"
};

const bottomRightStyle: React.CSSProperties = {
    bottom: "5px",
    right: "5px",
    position: "absolute",
    color: "white"
};

interface CornerstoneElementProps {
    stack: any;
}

interface CornerstoneElementState {
    element: HTMLDivElement;
    stack: any;
    viewport: any;
    imageId: any;
}

export default class CornerstoneElement extends React.Component<CornerstoneElementProps, CornerstoneElementState> {
    constructor(props) {
        super(props);
        this.state = {
            element: undefined,
            stack: props.stack,
            viewport: cornerstone.getDefaultViewport(null, undefined),
            imageId: props.stack.imageIds[0]
        };

        this.onImageRendered = this.onImageRendered.bind(this);
        this.onNewImage = this.onNewImage.bind(this);
        this.onWindowResize = this.onWindowResize.bind(this);
    }

    render() {
        return (
            <div>
                <div
                    className="viewportElement"
                    style={divStyle}
                    ref={input => {
                        this.setState({element: input});
                    }}
                >
                    <canvas className="cornerstone-canvas" />
                    <div style={bottomLeftStyle}>
                        Zoom: {this.state.viewport.scale}
                    </div>
                    <div style={bottomRightStyle}>
                        WW/WC: {this.state.viewport.voi.windowWidth} /{" "}
                        {this.state.viewport.voi.windowCenter}
                    </div>
                </div>
            </div>
        );
    }

    onWindowResize() {
        console.log("onWindowResize");
        cornerstone.resize(this.state.element);
    }

    onImageRendered() {
        const viewport = cornerstone.getViewport(this.state.element);
        console.log(viewport);

        this.setState({
            viewport
        });

        console.log(this.state.viewport);
    }

    onNewImage() {
        const enabledElement = cornerstone.getEnabledElement(this.state.element);

        this.setState({
            imageId: enabledElement.image.imageId
        });
    }

    componentDidMount() {
        const element = this.state.element;

        // Enable the DOM Element for use with Cornerstone
        cornerstone.enable(element);

        // Load the first image in the stack
        cornerstone.loadImage(this.state.imageId).then(image => {
            // Display the first image
            cornerstone.displayImage(element, image);

            // Add the stack tool state to the enabled element
            const stack = this.props.stack;
            cornerstoneTools.addStackStateManager(element, ["stack"]);
            cornerstoneTools.addToolState(element, "stack", stack);

            cornerstoneTools.mouseInput.enable(element);
            cornerstoneTools.mouseWheelInput.enable(element);
            cornerstoneTools.wwwc.activate(element, 1); // ww/wc is the default tool for left mouse button
            cornerstoneTools.pan.activate(element, 2); // pan is the default tool for middle mouse button
            cornerstoneTools.zoom.activate(element, 4); // zoom is the default tool for right mouse button
            cornerstoneTools.zoomWheel.activate(element); // zoom is the default tool for middle mouse wheel

            cornerstoneTools.touchInput.enable(element);
            cornerstoneTools.panTouchDrag.activate(element);
            cornerstoneTools.zoomTouchPinch.activate(element);

            element.addEventListener(
                "cornerstoneimagerendered",
                this.onImageRendered
            );
            element.addEventListener("cornerstonenewimage", this.onNewImage);
            window.addEventListener("resize", this.onWindowResize);
        });
    }

    componentWillUnmount() {
        const element = this.state.element;
        element.removeEventListener(
            "cornerstoneimagerendered",
            this.onImageRendered
        );

        element.removeEventListener("cornerstonenewimage", this.onNewImage);

        window.removeEventListener("resize", this.onWindowResize);

        cornerstone.disable(element);
    }

    componentDidUpdate(prevProps, prevState) {
        const stackData = cornerstoneTools.getToolState(this.state.element, "stack");
        const stack = stackData.data[0];
        stack.currentImageIdIndex = this.state.stack.currentImageIdIndex;
        stack.imageIds = this.state.stack.imageIds;
        cornerstoneTools.addToolState(this.state.element, "stack", stack);

        // const imageId = stack.imageIds[stack.currentImageIdIndex];
        // cornerstoneTools.scrollToIndex(this.element, stack.currentImageIdIndex);
    }
}

// const stack = {
//     imageIds: [imageId],
//     currentImageIdIndex: 0
// };

// const App = () => (
//     <div>
//         <h2>Cornerstone React Component Example</h2>
//         <CornerstoneElement stack={{ ...stack }} />
//     </div>
// );

// render(<App />, document.getElementById("root"));

