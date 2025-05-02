/**
 * NavMenuDrag.js - Handles drag and drop functionality for the project navigation menu with insertion line indicator
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

        // Get the dragged ID
        const draggedId = window.navMenuDrag.draggedId;
        if (!draggedId) return;

        // Remove existing insertion line
        this.removeInsertionLine();

        // Find the projects list container
        const projectsList = document.querySelector('.projects-list');
        if (!projectsList) return;

        // Find all project items
        const projectItems = Array.from(projectsList.querySelectorAll('.project-item'));

        // Find insertion point based on mouse position
        let insertIndex = projectItems.length;

        for (let i = 0; i < projectItems.length; i++) {
            const item = projectItems[i];
            // Skip the dragged item itself
            if (item.getAttribute('data-id') === draggedId) continue;

            const rect = item.getBoundingClientRect();
            const middle = rect.top + rect.height / 2;

            if (e.clientY < middle) {
                // Insert before this item
                this.createInsertionLine(projectsList, item, true);
                insertIndex = i;
                break;
            }
        }

        // If no insertion point found, append at end
        if (insertIndex === projectItems.length) {
            const lastItem = projectItems[projectItems.length - 1];
            // Don't show indicator if we're already at the bottom and that's the dragged item
            if (lastItem && lastItem.getAttribute('data-id') !== draggedId) {
                this.createInsertionLine(projectsList, null, false);
            }
        }
    },

    /**
     * Create an insertion line indicator
     * @param {Element} container - The container element
     * @param {Element} beforeElement - Element to insert before (null for end)
     * @param {boolean} insertBefore - Whether to insert before or after the element
     */
    createInsertionLine: function(container, beforeElement, insertBefore) {
        // Create the indicator element
        const indicator = document.createElement('div');
        indicator.id = 'nav-insertion-indicator';
        indicator.style.height = '4px';
        indicator.style.backgroundColor = '#009966'; // Same green color as KanbanBoard
        indicator.style.margin = '4px 0';
        indicator.style.borderRadius = '2px';
        indicator.style.width = '90%';
        indicator.style.marginLeft = '5%';

        if (beforeElement) {
            if (insertBefore) {
                container.insertBefore(indicator, beforeElement);
            } else {
                const nextElement = beforeElement.nextElementSibling;
                if (nextElement) {
                    container.insertBefore(indicator, nextElement);
                } else {
                    container.appendChild(indicator);
                }
            }
        } else {
            container.appendChild(indicator);
        }
    },

    /**
     * Remove the insertion line indicator
     */
    removeInsertionLine: function() {
        const indicator = document.getElementById('nav-insertion-indicator');
        if (indicator) indicator.remove();
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

        // Remove the insertion line
        this.removeInsertionLine();

        // Get the stored ID
        const draggedId = window.navMenuDrag.draggedId;
        if (!draggedId) {
            console.error("No dragged ID found");
            return;
        }

        // Find the projects list and items
        const projectsList = document.querySelector('.projects-list');
        const projectItems = Array.from(projectsList.querySelectorAll('.project-item'));

        // Find the dragged item's original index
        const draggedItem = projectItems.find(item => item.getAttribute('data-id') === draggedId);
        const sourceIndex = projectItems.indexOf(draggedItem);

        // Determine target index based on mouse position
        let targetIndex = projectItems.length;

        for (let i = 0; i < projectItems.length; i++) {
            // Skip the dragged item itself
            if (i === sourceIndex) continue;

            const rect = projectItems[i].getBoundingClientRect();
            const middle = rect.top + rect.height / 2;

            if (e.clientY < middle) {
                targetIndex = i;
                break;
            }
        }

        // If dropping at the same position, do nothing
        if (targetIndex === sourceIndex ||
            (sourceIndex < targetIndex && targetIndex === sourceIndex + 1)) {
            console.log("Dropping in the same position, ignoring");
            return;
        }

        console.log("Drop details:", {
            sourceId: draggedId,
            sourceIndex: sourceIndex,
            targetIndex: targetIndex
        });

        // Call the .NET method with the correct indices
        if (window.navMenuDrag.dotNetRef) {
            window.navMenuDrag.dotNetRef.invokeMethodAsync('HandleProjectDrop', draggedId, targetIndex)
                .then(() => {
                    console.log("HandleProjectDrop completed successfully");
                })
                .catch(error => {
                    console.error("Error in HandleProjectDrop:", error);
                });
        }
    },

    /**
     * Handle end of drag operation
     * @param {DragEvent} e - The drag event
     */
    handleDragEnd: function (e) {
        // Remove insertion line
        this.removeInsertionLine();

        // Clean up all styling
        document.querySelectorAll('.project-item').forEach(item => {
            item.classList.remove('dragging');
            item.classList.remove('drag-over');
        });

        // Reset stored values
        window.navMenuDrag.currentDropTarget = null;
        window.navMenuDrag.draggedId = null;
    }
};