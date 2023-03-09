const FinanceSheet = artifacts.require("FinanceSheet");

module.exports = function (deployer) {
  deployer.deploy(FinanceSheet, "Hello Blockchain");
};