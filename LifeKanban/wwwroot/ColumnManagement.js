/**
 * ColumnManagement.js - Handles drag and drop functionality for column management
 */
window.setupColumnDrag = function (dotNetRef) {
    console.log("Setting up column drag and drop");

    let draggedElement = null;
    let draggedId = null;

    // Helper function to find ancestor with class
    function findAncestor(element, className) {
        while (element && !element.classList.contains(className)) {
            element = element.parentElement;
        }
        return element;
    }

    // Add event listeners to the document to catch all drag events
    document.addEventListener('dragstart', function(e) {
        // Find the draggable column item
        const columnItem = findAncestor(e.target, 'column-management-item');

        if (columnItem && columnItem.getAttribute('draggable') === 'true') {
            draggedElement = columnItem;
            draggedId = columnItem.getAttribute('data-id');

            console.log("Drag started on:", draggedId);

            // Set data transfer (required for Firefox)
            e.dataTransfer.setData('text/plain', draggedId);
            e.dataTransfer.effectAllowed = 'move';

            // Add dragging class after a small delay to ensure it's applied
            setTimeout(() => {
                columnItem.classList.add('dragging');
            }, 0);
        }
    });

    document.addEventListener('dragend', function(e) {
        if (draggedElement) {
            console.log("Drag ended");
            draggedElement.classList.remove('dragging');

            // Remove drag-over class from all items
            document.querySelectorAll('.column-management-item').forEach(item => {
                item.classList.remove('drag-over');
            });

            draggedElement = null;
            draggedId = null;
        }
    });

    document.addEventListener('dragover', function(e) {
        // Find the column item being dragged over
        const columnItem = findAncestor(e.target, 'column-management-item');

        if (columnItem && draggedElement && columnItem !== draggedElement &&
            columnItem.getAttribute('draggable') === 'true') {

            e.preventDefault(); // Required to allow dropping
            e.dataTransfer.dropEffect = 'move';

            // Remove drag-over class from all items
            document.querySelectorAll('.column-management-item').forEach(item => {
                if (item !== columnItem) {
                    item.classList.remove('drag-over');
                }
            });

            // Add drag-over class to current target
            columnItem.classList.add('drag-over');
        }
    });

    document.addEventListener('drop', function(e) {
        // Find the column item being dropped on
        const columnItem = findAncestor(e.target, 'column-management-item');

        if (columnItem && draggedElement && columnItem !== draggedElement &&
            columnItem.getAttribute('draggable') === 'true') {

            e.preventDefault();

            console.log("Drop event on:", columnItem.getAttribute('data-id'));

            // Calculate target index
            const columnList = columnItem.parentElement;
            const items = Array.from(columnList.querySelectorAll('.column-management-item[draggable="true"]'));
            const targetIndex = items.indexOf(columnItem);

            console.log("Target index:", targetIndex);

            // Call .NET method to update order
            if (dotNetRef) {
                console.log("Invoking UpdateColumnOrder with:", draggedId, targetIndex);
                dotNetRef.invokeMethodAsync('UpdateColumnOrder', draggedId, targetIndex)
                    .then(() => {
                        console.log("Order updated successfully");
                    })
                    .catch(err => {
                        console.error("Error updating order:", err);
                    });
            } else {
                console.error("No .NET reference available");
            }

            // Remove drag-over class
            columnItem.classList.remove('drag-over');
        }
    });

    // Return a cleanup function
    return function() {
        console.log("Cleaning up column drag and drop");
        document.removeEventListener('dragstart');
        document.removeEventListener('dragend');
        document.removeEventListener('dragover');
        document.removeEventListener('drop');
    };
};