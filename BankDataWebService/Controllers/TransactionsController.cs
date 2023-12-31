﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankDataWebService.Data;
using BankDataWebService.Models;

namespace BankDataWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly DBManager _context;

        public TransactionsController(DBManager context)
        {
            _context = context;
        }

        // GET: api/Transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransaction()
        {
          if (_context.Transaction == null)
          {
              return NotFound();
          }
            return await _context.Transaction.ToListAsync();
        }

        // GET: api/Transactions/5
        [HttpGet("{accountNumber}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int accountNumber)
        {
          if (_context.Transaction == null)
          {
              return NotFound();
          }
            var transactions = await _context.Transaction.Where(a => a.accountNumber == accountNumber).ToListAsync();

            if (transactions == null)
            {
                return NotFound();
            }

            return Ok(transactions);
        }


        // POST: api/Transactions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Transaction>> PostTransaction(Transaction transaction)
        {
          if (_context.Transaction == null)
          {
              return Problem("Entity set 'DBManager.Transaction'  is null.");
          }
            if (!_context.Banks.Any(e => e.accountNumber == transaction.accountNumber)) 
            {
                return NotFound("(From) Account number not found.");
            }
            if (!_context.Banks.Any(e => e.accountNumber == transaction.toAccountNumber))
            {
                return NotFound("(To) Account number not found.");
            }
            if (transaction.amount <= 0)
            {
                return Problem("Invalid amount!");
            }
            Bank bank = await _context.Banks.FindAsync(transaction.accountNumber);
            Bank toBank = await _context.Banks.FindAsync(transaction.toAccountNumber);
            bank.balance -= transaction.amount;
            toBank.balance += transaction.amount;
            _context.Banks.Update(bank);
            _context.Banks.Update(toBank);
            _context.Transaction.Add(transaction);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTransaction", new { id = transaction.Id }, transaction);
        }

        // DELETE: api/Transactions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            if (_context.Transaction == null)
            {
                return NotFound();
            }
            var transaction = await _context.Transaction.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            _context.Transaction.Remove(transaction);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TransactionExists(int id)
        {
            return (_context.Transaction?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
