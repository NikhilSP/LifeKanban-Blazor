/**
 * NavMenuDrag.js - Handles drag and drop functionality for the project navigation menu
 */
window.navMenuDrag = {
    // Store these variables on the global object to avoid 'this' context issues
    draggedId: null,
    currentDropTarget: null,
    dotNetRef: null,

    /**
     * Initialize the drag and drop functionality
     * @param {DotNetObjectReference} dotNetRef - Reference to the Blazor component
     */
    setup: function (dotNetRef) {
        console.log("Setting up nav menu drag and drop");

        // Store the .NET reference
        window.navMenuDrag.dotNetRef = dotNetRef;

        // Find the projects list container
        const projectsList = document.querySelector('.projects-list');
        if (!projectsList) {
            console.error("Could not find projects-list element");
            return;
        }

        // Attach event listeners using a stable reference to 'this'
        const self = window.navMenuDrag;

        projectsList.addEventListener('dragstart', function(e) {
            self.handleDragStart(e);
        });

        projectsList.addEventListener('dragover', function(e) {
            e.preventDefault(); // Always prevent default to enable dropping
            self.handleDragOver(e);
        });

        projectsList.addEventListener('dragenter', function(e) {
            self.handleDragEnter(e);
        });

        projectsList.addEventListener('dragleave', function(e) {
            self.handleDragLeave(e);
        });

        projectsList.addEventListener('drop', function(e) {
            e.preventDefault();
            e.stopPropagation();
            self.handleDrop(e);
        });

        projectsList.addEventListener('dragend', function(e) {
            self.handleDragEnd(e);
        });

        console.log("Nav menu drag-and-drop initialized successfully");
    },

    /**
     * Handle the start of a drag operation
     * @param {DragEvent} e - The drag event
     */
    handleDragStart: function (e) {
        // Find the project item being dragged
        const projectItem = e.target.closest('.project-item');
        if (!projectItem) {
            console.error("No project item found in drag start");
            return;
        }

        // Get and store the project ID
        const id = projectItem.getAttribute('data-id');
        console.log("Starting drag on project with ID:", id);

        if (!id) {
            console.error("Project item is missing data-id attribute");
            return;
        }

        // Store the ID on the global object to maintain it across events
        window.navMenuDrag.draggedId = id;

        // Style the dragged item
        projectItem.classList.add('dragging');

        // Set data transfer properties (backup mechanism for ID)
        e.dataTransfer.effectAllowed = 'move';
        e.dataTransfer.setData('text/plain', id);

        // For debugging
        console.log("Drag started, stored ID:", window.navMenuDrag.draggedId);
    },

    /**
     * Handle dragging over potential drop targets
     * @param {DragEvent} e - The drag event
     */
    handleDragOver: function (e) {
        // Default is already prevented in the event listener
        e.dataTransfer.dropEffect = 'move';
    },

    /**
     * Handle entering a potential drop target
     * @param {DragEvent} e - The drag event
     */
    handleDragEnter: function (e) {
        const item = e.target.closest('.project-item');
        if (!item) return;

        // Skip if entering the item being dragged
        if (item.getAttribute('data-id') === window.navMenuDrag.draggedId) return;

        // Remove highlight from previous target
        if (window.navMenuDrag.currentDropTarget &&
            window.navMenuDrag.currentDropTarget !== item) {
            window.navMenuDrag.currentDropTarget.classList.remove('drag-over');
        }

        // Set and highlight new target
        window.navMenuDrag.currentDropTarget = item;
        item.classList.add('drag-over');

        e.stopPropagation();
    },

    /**
     * Handle leaving a potential drop target
     * @param {DragEvent} e - The drag event
     */
    handleDragLeave: function (e) {
        // Find the item being left
        const item = e.target.closest('.project-item');
        if (!item) return;

        // Check if we're truly leaving this item or just moving to a child element
        const relatedTarget = e.relatedTarget;
        if (!relatedTarget || (!item.contains(relatedTarget) && relatedTarget !== item)) {
            item.classList.remove('drag-over');

            if (window.navMenuDrag.currentDropTarget === item) {
                window.navMenuDrag.currentDropTarget = null;
            }
        }

        e.stopPropagation();
    },

    /**
     * Handle dropping the dragged item
     * @param {DragEvent} e - The drag event
     */
    handleDrop: function (e) {
        console.log("Drop event triggered");

        // Get the stored ID from the global object
        const draggedId = window.navMenuDrag.draggedId;
        console.log("Using stored draggedId:", draggedId);

        // Fallback to dataTransfer if our stored ID is missing
        let sourceId = draggedId;
        if (!sourceId) {
            sourceId = e.dataTransfer.getData('text/plain');
            console.log("Fallback to dataTransfer ID:", sourceId);
        }

        // Validate we have an ID
        if (!sourceId) {
            console.error("No valid source ID for drop operation");
            return;
        }

        // Find the drop target
        const dropTarget = e.target.closest('.project-item');
        if (!dropTarget) {
            console.error("No valid drop target found");
            return;
        }

        const dropTargetId = dropTarget.getAttribute('data-id');

        // Don't drop onto self
        if (sourceId === dropTargetId) {
            console.log("Dropped onto self, ignoring");
            return;
        }

        // Clear highlight
        if (window.navMenuDrag.currentDropTarget) {
            window.navMenuDrag.currentDropTarget.classList.remove('drag-over');
            window.navMenuDrag.currentDropTarget = null;
        }

        // Get target index
        const projectItems = Array.from(document.querySelectorAll('.project-item'));
        const targetIndex = projectItems.indexOf(dropTarget);

        console.log("Drop details:", {
            sourceId: sourceId,
            targetIndex: targetIndex,
            dropTargetId: dropTargetId
        });

        // Apply animation before making the actual change
        this.animateReordering(sourceId, targetIndex);

        // Call the .NET method with a short delay to allow animation to start
        setTimeout(() => {
            if (window.navMenuDrag.dotNetRef) {
                console.log("Calling .NET HandleProjectDrop with:", sourceId, targetIndex);

                window.navMenuDrag.dotNetRef.invokeMethodAsync('HandleProjectDrop', sourceId, targetIndex)
                    .then(() => {
                        console.log("HandleProjectDrop completed successfully");
                    })
                    .catch(error => {
                        console.error("Error in HandleProjectDrop:", error);
                    });
            } else {
                console.error("No .NET reference available");
            }
        }, 50);
    },

    /**
     * Handle end of drag operation
     * @param {DragEvent} e - The drag event
     */
    handleDragEnd: function (e) {
        // Clean up all styling
        document.querySelectorAll('.project-item').forEach(item => {
            item.classList.remove('dragging');
            item.classList.remove('drag-over');
        });

        // For debugging
        console.log("Drag ended, final draggedId was:", window.navMenuDrag.draggedId);

        // Reset stored values
        window.navMenuDrag.currentDropTarget = null;
        window.navMenuDrag.draggedId = null;
    },

    /**
     * Animate the reordering of projects
     * @param {string} sourceId - ID of the project being moved
     * @param {number} targetIndex - Target position index
     */
    animateReordering: function(sourceId, targetIndex) {
        const projectItems = Array.from(document.querySelectorAll('.project-item'));
        const sourceElement = document.querySelector(`.project-item[data-id="${sourceId}"]`);

        if (!sourceElement) {
            console.error("Source element not found for animation");
            return;
        }

        const sourceIndex = projectItems.indexOf(sourceElement);
        console.log(`Animating: moving from index ${sourceIndex} to ${targetIndex}`);

        // Apply animations based on movement direction
        if (sourceIndex < targetIndex) {
            // Moving down - items in between move up
            for (let i = sourceIndex + 1; i <= targetIndex; i++) {
                if (projectItems[i]) {
                    projectItems[i].classList.add('move-up');
                    setTimeout(() => {
                        projectItems[i].classList.remove('move-up');
                    }, 300);
                }
            }
        } else {
            // Moving up - items in between move down
            for (let i = targetIndex; i < sourceIndex; i++) {
                if (projectItems[i]) {
                    projectItems[i].classList.add('move-down');
                    setTimeout(() => {
                        projectItems[i].classList.remove('move-down');
                    }, 300);
                }
            }
        }
    }
};