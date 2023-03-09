// SPDX-License-Identifier: GPL-3.0

pragma solidity ^0.8.1;

contract FinanceSheet {
    struct Transaction {
        uint amount;
        uint date;
        string description;
        bool isExpense;
    }

    struct ChecklistItem {
        uint amount;
        uint dueDate;
        string description;
        bool paid;
    }

    mapping(address => Transaction[]) private transactions;
    mapping(address => ChecklistItem[]) private checklistItems;
    mapping(address => uint) private transactionCount;
    mapping(address => uint) private checklistItemCount;

    function addTransaction(
        uint _amount,
        uint _date,
        string memory _description,
        bool _isExpense
    ) public {
        transactions[msg.sender].push(
            Transaction(_amount, _date, _description, _isExpense)
        );
        transactionCount[msg.sender]++;
    }

    function getTransactionCount() public view returns (uint) {
        return transactionCount[msg.sender];
    }

    function getTransaction(
        uint _index
    ) public view returns (uint, uint, string memory, bool) {
        require(
            _index < transactionCount[msg.sender],
            "Invalid transaction index"
        );
        Transaction storage transaction = transactions[msg.sender][_index];
        return (
            transaction.amount,
            transaction.date,
            transaction.description,
            transaction.isExpense
        );
    }

    function addChecklistItem(
        uint _amount,
        uint _dueDate,
        string memory _description
    ) public {
        checklistItems[msg.sender].push(
            ChecklistItem(_amount, _dueDate, _description, false)
        );
        checklistItemCount[msg.sender]++;
    }

    function getChecklistItemCount() public view returns (uint) {
        return checklistItemCount[msg.sender];
    }

    function getChecklistItem(
        uint _index,
        bool _unpaidOnly
    ) public view returns (uint, uint, string memory, bool) {
        require(
            _index < checklistItemCount[msg.sender],
            "Invalid checklist item index"
        );
        ChecklistItem storage item = checklistItems[msg.sender][_index];
        if (_unpaidOnly && item.paid) {
            return (0, 0, "", true);
        } else {
            return (item.amount, item.dueDate, item.description, item.paid);
        }
    }

    function markChecklistItemPaid(uint _index) public {
        require(
            _index < checklistItemCount[msg.sender],
            "Invalid checklist item index"
        );
        ChecklistItem storage item = checklistItems[msg.sender][_index];
        require(!item.paid, "Checklist item already paid");
        item.paid = true;
    }
}
