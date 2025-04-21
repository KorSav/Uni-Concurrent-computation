package queueing_system;

import java.util.ArrayList;
import java.util.concurrent.locks.Condition;
import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;

public class SemiBlockingQueue<T> {
    private final ArrayList<T> _list;
    private final int _cap;
    private boolean _isTerminated = false;
    private final Lock _lock = new ReentrantLock();
    private final Condition _condition = _lock.newCondition();

    public SemiBlockingQueue(int size) {
        _list = new ArrayList<>(size);
        _cap = size;
    }

    public int size() {
        _lock.lock();
        try {
            return _list.size();
        } finally {
            _lock.unlock();
        }
    }

    public boolean isTerminated() {
        _lock.lock();
        try {
            return _isTerminated;
        } finally {
            _lock.unlock();
        }
    }

    /**
     * Returns boolean identifying whether object was queued
     */
    public boolean put(T obj) {
        _lock.lock();
        try {
            if (_isTerminated) {
                throw new IllegalStateException(
                        "Forbidden to put elements when queue is terminated.");
            }
            if (_list.size() == _cap) {
                return false;
            }
            _list.add(obj);
            _condition.signalAll();
        } finally {
            _lock.unlock();
        }
        return true;
    }

    /**
     * If queue has elements - returns first one
     * <p>
     * If queue is empty and terminated - returns null
     * <p>
     * If queue is empty - waits for new object
     */
    public T take() throws InterruptedException {
        T obj;
        _lock.lock();
        try {
            while (_list.isEmpty()) {
                if (_isTerminated) {
                    _lock.unlock();
                    return null;
                }
                _condition.await();
            }
            obj = _list.removeFirst();
        } finally {
            _lock.unlock();
        }
        return obj;
    }

    public void terminate() {
        _lock.lock();
        try {
            _isTerminated = true;
            _condition.signalAll();
        } finally {
            _lock.unlock();
        }
    }
}
